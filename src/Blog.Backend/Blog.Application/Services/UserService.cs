using System.Linq.Expressions;
using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Request.User;
using Blog.Application.Models.Response;
using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Services;

public class UserService : IUserService
{
    private readonly IMomentProvider _momentProvider;
    private readonly IEncryptionProvider _encryptionProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepository;

    public UserService(IMomentProvider momentProvider, IEncryptionProvider encryptionProvider, IUnitOfWork unitOfWork)
    {
        _momentProvider = momentProvider;
        _encryptionProvider = encryptionProvider;
        _unitOfWork = unitOfWork;
        _userRepository = unitOfWork.GetRepository<User>();
    }

    public async Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.AnyAsync(user => user.Username == username,
                cancellationToken))
        {
            return ErrorCode.UsernameAlreadyExists;
        }

        if (await _userRepository.AnyAsync(user => user.Email == email,
                cancellationToken))
        {
            return ErrorCode.EmailAlreadyExists;
        }

        var hashedPassword = _encryptionProvider.HashPassword(password);

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword.Hash,
            PasswordSalt = hashedPassword.Salt,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new None();
    }

    public async Task<Result<UserModel>> CreateGoogleAsync(CreateGoogleRequestModel createGoogleRequestModel,
        CancellationToken cancellationToken = default)
    {
        var hashedPassword = _encryptionProvider.HashPassword(createGoogleRequestModel.Password);

        var newUser = new User
        {
            Email = createGoogleRequestModel.Email,
            Username = createGoogleRequestModel.Username,
            PasswordSalt = hashedPassword.Salt,
            PasswordHash = hashedPassword.Hash,
            ProfilePictureUrl = createGoogleRequestModel.PictureUrl,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newUser.ToModel();
    }

    public async Task<Result<UserModel>> GetCheckedUserAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var validUser = await _userRepository.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

        if (validUser is null)
        {
            return ErrorCode.InvalidCredentials;
        }

        var validUserPasswordSaltAndHash = new SaltAndHash(validUser.PasswordSalt, validUser.PasswordHash);

        if (!_encryptionProvider.VerifyPasswordHash(password, validUserPasswordSaltAndHash))
        {
            return ErrorCode.InvalidCredentials;
        }

        return validUser.ToModel();
    }

    public async Task<Result<UserModel>> UpdateAsync(int userId, UpdateUserRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (existingUser is null)
        {
            return ErrorCode.UserNotFound;
        }

        if (!string.IsNullOrEmpty(request.CurrentPassword) && !string.IsNullOrEmpty(request.NewPassword))
        {
            var currentUserPassword = new SaltAndHash(existingUser.PasswordSalt, existingUser.PasswordHash);

            if (!_encryptionProvider.VerifyPasswordHash(request.CurrentPassword, currentUserPassword))
            {
                return ErrorCode.PasswordDontMatch;
            }

            if (_encryptionProvider.VerifyPasswordHash(request.NewPassword!, currentUserPassword))
            {
                return ErrorCode.InvalidCredentials;
            }

            var newUserPassword = _encryptionProvider.HashPassword(request.NewPassword!);

            existingUser.PasswordSalt = newUserPassword.Salt;
            existingUser.PasswordHash = newUserPassword.Hash;
        }

        if (request.Username != existingUser.Username && !string.IsNullOrEmpty(request.Username))
        {
            if (await _userRepository.AnyAsync(user => user.Username == request.Username, cancellationToken))
            {
                return ErrorCode.UsernameAlreadyExists;
            }

            existingUser.Username = request.Username;
        }

        if (request.Email != existingUser.Email && !string.IsNullOrEmpty(request.Email))
        {
            if (await _userRepository.AnyAsync(user => user.Email == request.Email, cancellationToken))
            {
                return ErrorCode.EmailAlreadyExists;
            }

            existingUser.Email = request.Email;
        }

        if (request.ProfilePictureUrl != existingUser.ProfilePictureUrl &&
            !string.IsNullOrEmpty(request.ProfilePictureUrl))
        {
            existingUser.ProfilePictureUrl = request.ProfilePictureUrl;
        }

        if (!_userRepository.IsModified(existingUser))
        {
            return ErrorCode.NothingToUpdate;
        }

        existingUser.UpdatedAt = _momentProvider.DateTimeOffsetUtcNow;

        _userRepository.Update(existingUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existingUser.ToModel();
    }

    public async Task<Result<None>> DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _userRepository.RemoveAsync(user => user.Id == userId, cancellationToken);

        return rowsAffected == 0 ? ErrorCode.NothingToDelete : new None();
    }

    public async Task<Result<GetUsersResponseModel>> GetAsync(GetUsersRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var usersQuery = _userRepository.AsQueryable();

        var lastMonthUsersCount = await usersQuery.Where(user =>
                user.CreatedAt >= _momentProvider.DateTimeOffsetUtcNow.AddMonths(-1))
            .CountAsync(cancellationToken);

        Expression<Func<User, object>> sortProperty = user => user.CreatedAt;

        usersQuery = request.SortOrder?.ToLower() == "desc"
            ? usersQuery.OrderByDescending(sortProperty)
            : usersQuery.OrderBy(sortProperty);

        var userModels = usersQuery.Select(user => user.ToModel());

        var startIndex = request.StartIndex ?? 0;
        var limit = request.Limit ?? 9;

        var responseItems = await PagedList<UserModel>.CreateAsync(
            userModels,
            startIndex,
            limit,
            cancellationToken);

        return new GetUsersResponseModel
        {
            Data = responseItems,
            LastMonthUsersCount = lastMonthUsersCount
        };
    }
}