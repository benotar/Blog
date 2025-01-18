using Blog.Application.Common;
using Blog.Application.Extensions;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Domain.Entities;

namespace Blog.Application.Services;

public class GoogleService : IGoogleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureTranslatorService _translatorService;
    private readonly IUserService _userService;

    public GoogleService(IUnitOfWork unitOfWork, IAzureTranslatorService translatorService, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _translatorService = translatorService;
        _userService = userService;
    }

    public async Task<Result<UserModel>> FindOrCreateGoogleUserAsync(string email, string name, string pictureUrl,
        CancellationToken cancellationToken = default)
    {
        var validUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(email, cancellationToken);

        if (validUser is not null)
        {
            return new UserModel
            {
                Id = validUser.Id,
                Email = validUser.Email,
                ProfilePictureUrl = validUser.ProfilePictureUrl,
                Username = validUser.Username
            };
        }

        var translatedName = await _translatorService.TranslateAsync(name);
        var username = translatedName.ToUsername();
        
        return await _userService.CreateGoogleAsync(username, email, username, pictureUrl, cancellationToken);
    }
}