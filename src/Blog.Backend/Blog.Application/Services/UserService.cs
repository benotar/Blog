﻿using Blog.Application.Common;
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

    public async Task<Result<UserModel>> CreateGoogleAsync(string name, string email, string pictureUrl,
        CancellationToken cancellationToken = default)
    {
        var hashedPassword = _encryptionProvider.HashPassword("askjhaskjkjhsadkjhd2989201");


        var newUser = new User
        {
            Email = email,
            Username = "dasda",
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
            Username = validUser.Username
        };
    }
}