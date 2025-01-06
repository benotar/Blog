using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
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

    public async Task<Result<None>> CreateAsync(string username, string email, string password, CancellationToken cancellationToken = default)
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
            CreatedAt = _momentProvider.Now
        };
        
        _unitOfWork.UserRepository.Add(newUser);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<None>.Success();
    }
}