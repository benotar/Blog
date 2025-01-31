using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services;

public class UserService : IUserService
{
    // readonly IUnitOfWorkTemp _unitOfWorkTempTemp;
    private readonly IMomentProvider _momentProvider;
    private readonly IEncryptionProvider _encryptionProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepository;

    public UserService(IMomentProvider momentProvider, IEncryptionProvider encryptionProvider, IUnitOfWork unitOfWork)
    {
        _momentProvider = momentProvider;
        //_unitOfWorkTempTemp = unitOfWorkTempTemp;
        _encryptionProvider = encryptionProvider;
        _unitOfWork = unitOfWork;
        _userRepository = unitOfWork.GetRepository<User>();
    }

    public async Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default)
    {
        var isUserExist = await _userRepository.AnyAsync(user => user.Email == email, cancellationToken);

        //var isUserExist = await _unitOfWorkTempTemp.UserRepository.AnyByEmailAsync(email, cancellationToken);

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

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        //_unitOfWorkTempTemp.UserRepository.Add(newUser);

        //await _unitOfWorkTempTemp.SaveChangesAsync(cancellationToken);

        return new None();
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

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);


        // _unitOfWorkTempTemp.UserRepository.Add(newUser);
        // await _unitOfWorkTempTemp.SaveChangesAsync(cancellationToken);

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
        //var validUser = await _unitOfWorkTempTemp.UserRepository.GetUserByEmailAsync(email, cancellationToken);

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

        return new UserModel
        {
            Id = validUser.Id,
            Email = validUser.Email,
            Username = validUser.Username,
            ProfilePictureUrl = validUser.ProfilePictureUrl
        };
    }

    public async Task<Result<UserModel>> UpdateAsync(int userId, string? username, string? email,
        string? profilePictureUrl, string? currentPassword, string? newPassword,
        CancellationToken cancellationToken = default)
    {
        // var existingUser = await _unitOfWorkTempTemp.UserRepository
        //     .GetByIdAsync(userId, cancellationToken);

        var existingUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (existingUser is null)
        {
            return ErrorCode.InvalidCredentials;
        }

        if (!string.IsNullOrEmpty(currentPassword) ^ !string.IsNullOrEmpty(newPassword))
        {
            return ErrorCode.EnterYourCurrentAndNewPassword;
        }

        var isPasswordUpdated = false;

        if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword))
        {
            var currentUserPassword = new SaltAndHash(existingUser.PasswordSalt, existingUser.PasswordHash);

            if (!_encryptionProvider.VerifyPasswordHash(currentPassword!, currentUserPassword))
            {
                return ErrorCode.PasswordDontMatch;
            }

            if (!_encryptionProvider.VerifyPasswordHash(newPassword!, currentUserPassword))
            {
                var newUserPassword = _encryptionProvider.HashPassword(newPassword!);

                existingUser.PasswordSalt = newUserPassword.Salt;
                existingUser.PasswordHash = newUserPassword.Hash;

                isPasswordUpdated = true;
            }
        }

        if (!isPasswordUpdated &&
            (string.IsNullOrEmpty(username) ||
             string.Equals(existingUser.Username, username, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrEmpty(email) ||
                string.Equals(existingUser.Email, email, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrEmpty(profilePictureUrl) || string.Equals(existingUser.ProfilePictureUrl,
                profilePictureUrl, StringComparison.OrdinalIgnoreCase)))
        {
            return ErrorCode.NothingToUpdate;
        }

        if (!string.IsNullOrEmpty(username)) existingUser.Username = username;

        if (!string.IsNullOrEmpty(email)) existingUser.Email = email;

        if (!string.IsNullOrEmpty(profilePictureUrl)) existingUser.ProfilePictureUrl = profilePictureUrl;

        existingUser.UpdatedAt = _momentProvider.DateTimeOffsetUtcNow;

        _userRepository.Update(existingUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        //await _unitOfWorkTempTemp.SaveChangesAsync(cancellationToken);

        return new UserModel
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            Username = existingUser.Username,
            ProfilePictureUrl = existingUser.ProfilePictureUrl
        };
    }

    public async Task<Result<None>> DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _userRepository.RemoveAsync(user => user.Id == userId, cancellationToken);
        //var rowsAffected = await _unitOfWorkTempTemp.UserRepository.DeleteAsync(userId, cancellationToken);

        return rowsAffected == 0 ? ErrorCode.NothingToDelete : new None();
    }
}