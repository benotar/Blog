using Blog.Application.Common;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;

namespace Blog.Application.Services;

public class GoogleService : IGoogleService
{
    private readonly IUnitOfWork _unitOfWork;

    public GoogleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserModel>> FindOrCreateGoogleUserAsync(string email, string name, string pictureUrl,
        CancellationToken cancellationToken = default)
    {
        var validUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(email, cancellationToken);

        return new UserModel
        {
            Id = validUser.Id,
            Email = validUser.Email,
            ProfilePictureUrl = validUser.ProfilePictureUrl,
            Username = validUser.Username
        };
    }
}