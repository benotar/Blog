using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Request.User;
using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

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
        var isUserExist = await _userRepository.AnyAsync(user => user.Email == email, cancellationToken);

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

    public async Task<Result<UserModel>> UpdateAsync(int userId, UpdateUserRequestModel updateUserRequestModel,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (existingUser is null)
        {
            return ErrorCode.InvalidCredentials;
        }

        if (!string.IsNullOrEmpty(updateUserRequestModel.CurrentPassword) ^
            !string.IsNullOrEmpty(updateUserRequestModel.NewPassword))
        {
            return ErrorCode.EnterYourCurrentAndNewPassword;
        }

        var isPasswordUpdated = false;

        if (!string.IsNullOrEmpty(updateUserRequestModel.CurrentPassword) &&
            !string.IsNullOrEmpty(updateUserRequestModel.NewPassword))
        {
            var currentUserPassword = new SaltAndHash(existingUser.PasswordSalt, existingUser.PasswordHash);

            if (!_encryptionProvider.VerifyPasswordHash(updateUserRequestModel.CurrentPassword!, currentUserPassword))
            {
                return ErrorCode.PasswordDontMatch;
            }

            if (!_encryptionProvider.VerifyPasswordHash(updateUserRequestModel.NewPassword!, currentUserPassword))
            {
                var newUserPassword = _encryptionProvider.HashPassword(updateUserRequestModel.NewPassword!);

                existingUser.PasswordSalt = newUserPassword.Salt;
                existingUser.PasswordHash = newUserPassword.Hash;

                isPasswordUpdated = true;
            }
        }

        if (!isPasswordUpdated &&
            (string.IsNullOrEmpty(updateUserRequestModel.Username) ||
             string.Equals(existingUser.Username, updateUserRequestModel.Username, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrEmpty(updateUserRequestModel.Email) ||
                string.Equals(existingUser.Email, updateUserRequestModel.Email, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrEmpty(updateUserRequestModel.ProfilePictureUrl) || string.Equals(
                existingUser.ProfilePictureUrl,
                updateUserRequestModel.ProfilePictureUrl, StringComparison.OrdinalIgnoreCase)))
        {
            return ErrorCode.NothingToUpdate;
        }

        if (!string.IsNullOrEmpty(updateUserRequestModel.Username))
            existingUser.Username = updateUserRequestModel.Username;

        if (!string.IsNullOrEmpty(updateUserRequestModel.Email)) existingUser.Email = updateUserRequestModel.Email;

        if (!string.IsNullOrEmpty(updateUserRequestModel.ProfilePictureUrl))
            existingUser.ProfilePictureUrl = updateUserRequestModel.ProfilePictureUrl;

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
}