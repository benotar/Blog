using System.Security.Cryptography;
using Blog.Application.Common;
using Blog.Application.Extensions;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;

namespace Blog.Application.Services;

public class GoogleService : IGoogleService
{
    private const int MaxUsernameRetries = 5;

    private readonly IRepository<User> _userRepository;
    private readonly IUserService _userService;

    public GoogleService(IUserService userService, IUnitOfWork unitOfWork)
    {
        _userService = userService;
        _userRepository = unitOfWork.GetRepository<User>();
    }

    public async Task<Result<UserModel>> FindOrCreateGoogleUserAsync(string email, string name, string pictureUrl,
        CancellationToken cancellationToken = default)
    {
        var validUser = await _userRepository.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

        if (validUser is not null)
        {
            return validUser.ToModel();
        }

        var username = await GenerateUniqueUsernameAsync(email, cancellationToken);

        return await _userService.CreateGoogleAsync(new CreateGoogleRequestModel
        {
            Username = username, Email = email, PictureUrl = pictureUrl, Password = username, // TODO: refactor to OAuth password
        }, cancellationToken);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string email, CancellationToken ct)
    {
        var basePart = email.ToUsernameBaseFromEmail();

        if (!await _userRepository.AnyAsync(u => u.Username == basePart, ct))
        {
            return basePart;
        }
        
        for (var i = 0; i < MaxUsernameRetries; i++)
        {
            var suffix = Random.Shared.Next(1000, 9999);
            var candidate = $"{basePart}_{suffix}";

            if (!await _userRepository.AnyAsync(u => u.Username == candidate, ct))
            {
                return candidate;
            }
        }

        var hexSuffix = Convert.ToHexString(
            RandomNumberGenerator.GetBytes(4)).ToLowerInvariant();
        return $"{basePart}_{hexSuffix}";
    }
}
