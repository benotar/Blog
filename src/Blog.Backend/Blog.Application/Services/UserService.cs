using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMomentProvider _momentProvider;
    private readonly IEncryptionProvider _encryptionProvider;

    public UserService(IMomentProvider momentProvider, IUnitOfWork unitOfWork, IEncryptionProvider encryptionProvider)
    {
        _momentProvider = momentProvider;
        _unitOfWork = unitOfWork;
        _encryptionProvider = encryptionProvider;
    }

    public async Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default)
    {
        var isUserExist = await _unitOfWork.UserRepository.AnyByEmailAsync(email, cancellationToken);

        if (isUserExist)
        {
            return ErrorCode.UserAlreadyExists;
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

        _unitOfWork.UserRepository.Add(newUser);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<None>.Success();
    }

    public async Task<Result<UserModel>> CreateGoogleAsync(string username, string email, string password,
        string pictureUrl,
        CancellationToken cancellationToken = default)
    {
        var hashedPassword = _encryptionProvider.HashPassword(password);

        var newUser = new User
        {
            Email = email,
            Username = username,
            PasswordSalt = hashedPassword.Salt,
            PasswordHash = hashedPassword.Hash,
            ProfilePictureUrl = pictureUrl,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        _unitOfWork.UserRepository.Add(newUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserModel
        {
            Id = newUser.Id,
            Email = newUser.Email,
            Username = newUser.Username,
            ProfilePictureUrl = newUser.ProfilePictureUrl,
        };
    }

    public async Task<Result<UserModel>> GetCheckedUserAsync(string email, string password,
        CancellationToken cancellationToken = default)
    {
        var validUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(email, cancellationToken);

        if (validUser is null)
        {
            return ErrorCode.InvalidCredentials;
        }

        var validUserPasswordSaltAndHash = new SaltAndHash(validUser.PasswordSalt, validUser.PasswordHash);

        if (!_encryptionProvider.VerifyPasswordHash(password, validUserPasswordSaltAndHash))
        {
            return ErrorCode.InvalidCredentials;
        }

        return new UserModel
        {
            Id = validUser.Id,
            Email = validUser.Email,
            Username = validUser.Username,
            ProfilePictureUrl = validUser.ProfilePictureUrl
        };
    }

    public async Task<Result<UserModel>> UpdateAsync(int userId, string? username, string? email,
        string? profilePictureUrl,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _unitOfWork.UserRepository
            .GetByIdAsync(userId, cancellationToken);

        if (existingUser is null)
        {
            return ErrorCode.InvalidCredentials;
        }

        if ((username is null || string.Equals(existingUser.Username, username, StringComparison.OrdinalIgnoreCase))
            && (email is null || string.Equals(existingUser.Email, email,
                StringComparison.OrdinalIgnoreCase))
            && (profilePictureUrl is null || string.Equals(existingUser.ProfilePictureUrl,
                profilePictureUrl, StringComparison.OrdinalIgnoreCase)))
        {
            return ErrorCode.NothingToUpdate;
        }

        if (username is not null)
        {
            existingUser.Username = username;
        }

        if (email is not null)
        {
            existingUser.Email = email;
        }
        
        if (profilePictureUrl is not null)
        {
            existingUser.ProfilePictureUrl = profilePictureUrl;
        }
        existingUser.UpdatedAt = _momentProvider.DateTimeOffsetUtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserModel
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            Username = existingUser.Username,
            ProfilePictureUrl = existingUser.ProfilePictureUrl
        };
        // var rowsAffected = await _unitOfWork.UserRepository
        //     .UpdateAsync(userId, username, email, profilePictureUrl, cancellationToken);
        //
        // return rowsAffected == 0 ? ErrorCode.NothingToUpdate : Result<None>.Success();
    }
}