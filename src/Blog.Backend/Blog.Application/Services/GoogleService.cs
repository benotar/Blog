using Blog.Application.Common;
using Blog.Application.Extensions;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;

namespace Blog.Application.Services;

public class GoogleService : IGoogleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepository;
    private readonly IAzureTranslatorService _translatorService;
    private readonly IUserService _userService;

    public GoogleService(IAzureTranslatorService translatorService, IUserService userService, IUnitOfWork unitOfWork)
    {
        _translatorService = translatorService;
        _userService = userService;
        _unitOfWork = unitOfWork;
        _userRepository = _unitOfWork.GetRepository<User>();
    }

    public async Task<Result<UserModel>> FindOrCreateGoogleUserAsync(string email, string name, string pictureUrl,
        CancellationToken cancellationToken = default)
    {
        var validUser = await _userRepository.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

        if (validUser is not null)
        {
            return validUser.ToModel();
        }

        var translatedName = await _translatorService.TranslateAsync(name);
        var username = translatedName.ToUsername();


        return await _userService.CreateGoogleAsync(new CreateGoogleRequestModel
        {
            Username = username,
            Email = email,
            PictureUrl = username,
            Password = pictureUrl,
        }, cancellationToken);
    }
}