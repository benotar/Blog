using System.Linq.Expressions;
using Blog.Application.Common;
using Blog.Application.Interfaces.FactoryMethod;
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
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepository;
    private readonly IPaginationFactory<UserModel> _userPaginationFactory;

    public UserService(IMomentProvider momentProvider, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork,
        IPaginationFactory<UserModel> userPaginationFactory)
    {
        _momentProvider = momentProvider;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _userPaginationFactory = userPaginationFactory;
        _userRepository = unitOfWork.GetRepository<User>();
    }

    public async Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default)
    {
        if (await _userRepository.AnyAsync(user => user.Username == username,
                cancellationToken))
        {
            return ErrorCode.UsernameAlreadyExist;
        }

        if (await _userRepository.AnyAsync(user => user.Email == email,
                cancellationToken))
        {
            return ErrorCode.EmailAlreadyExist;
        }

        var hashedPassword = _passwordHasher.HashPassword(password);

        var newUser = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hashedPassword,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new None();
    }

    public async Task<Result<UserModel>> CreateGoogleAsync(CreateGoogleRequestModel createGoogleRequestModel,
        CancellationToken cancellationToken = default)
    {
        var hashedPassword = _passwordHasher.HashPassword(createGoogleRequestModel.Password);

        var newUser = new User
        {
            Email = createGoogleRequestModel.Email,
            Username = createGoogleRequestModel.Username,
            PasswordHash = hashedPassword,
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

        if (validUser is null || !_passwordHasher.VerifyPasswordHash(password, validUser.PasswordHash))
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
            if (!_passwordHasher.VerifyPasswordHash(request.CurrentPassword, existingUser.PasswordHash))
            {
                return ErrorCode.PasswordDontMatch;
            }

            if (_passwordHasher.VerifyPasswordHash(request.NewPassword!, existingUser.PasswordHash))
            {
                return ErrorCode.InvalidCredentials;
            }

            var newUserPasswordHash = _passwordHasher.HashPassword(request.NewPassword!);

            existingUser.PasswordHash = newUserPasswordHash;
        }

        if (request.Username != existingUser.Username && !string.IsNullOrEmpty(request.Username))
        {
            if (await _userRepository.AnyAsync(user => user.Username == request.Username, cancellationToken))
            {
                return ErrorCode.UsernameAlreadyExist;
            }

            existingUser.Username = request.Username;
        }

        if (request.Email != existingUser.Email && !string.IsNullOrEmpty(request.Email))
        {
            if (await _userRepository.AnyAsync(user => user.Email == request.Email, cancellationToken))
            {
                return ErrorCode.EmailAlreadyExist;
            }

            existingUser.Email = request.Email;
        }

        if (request.ProfilePictureUrl != existingUser.ProfilePictureUrl && !string.IsNullOrEmpty(request.ProfilePictureUrl))
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
        var usersQuery = _userRepository.AsNoTracking();

        var lastMonthUsersCount = await usersQuery.Where(user =>
                user.CreatedAt >= _momentProvider.DateTimeOffsetUtcNow.AddMonths(-1))
            .CountAsync(cancellationToken);

        Expression<Func<User, object>> sortProperty = user => user.CreatedAt;

        usersQuery = request.SortOrder?.ToLower() == "desc"
            ? usersQuery.OrderByDescending(sortProperty)
            : usersQuery.OrderBy(sortProperty);

        var userModels = usersQuery.Select(user => new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        });

        var responseItems = await _userPaginationFactory
            .CreatePaginationFactory(PaginationType.Offset)
            .CreatePagedListAsync(
                userModels,
                request.StartIndex,
                request.Limit,
                cancellationToken);

        return new GetUsersResponseModel { Data = responseItems, LastMonthUsersCount = lastMonthUsersCount };
    }
}
