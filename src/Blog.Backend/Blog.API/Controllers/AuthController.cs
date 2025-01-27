using Blog.API.Models.Request;
using Blog.API.Models.Response;
using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

public class AuthController : BaseController
{
    private readonly IUserService _userService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IGoogleService _googleService;

    public AuthController(IUserService userService, IJwtProvider jwtProvider, IGoogleService googleService)
    {
        _userService = userService;
        _jwtProvider = jwtProvider;
        _googleService = googleService;
    }

    [HttpPost("sign-up")]
    public async Task<Result<None>> SignUp([FromBody] SignUpRequestModel model, CancellationToken cancellationToken)
    {
        return await _userService
            .CreateAsync(model.Username, model.Email, model.Password, cancellationToken);
    }

    [HttpPost("sign-in")]
    public async Task<Result<SignInResponseModel>> Login([FromBody] SignInRequestModel model,
        CancellationToken cancellationToken)
    {
        var validUserResult = await _userService
            .GetCheckedUserAsync(model.Email, model.Password, cancellationToken);

        if (!validUserResult.IsSucceed)
        {
            return validUserResult.ErrorCode;
        }

        var validUser = validUserResult.Payload;

        var accessToken = _jwtProvider.GenerateToken(validUser);
        var refreshToken = await _jwtProvider.CreateRefreshTokenAsync(validUser, cancellationToken);

        return new SignInResponseModel
        {
            Id = validUser.Id,
            Email = validUser.Email,
            Username = validUser.Username,
            ProfilePictureUrl = validUser.ProfilePictureUrl,
            Tokens = new TokensResponseModel(accessToken, refreshToken)
        };
    }

    [HttpPost("google")]
    public async Task<Result<GoogleSignInResponseModel>> GoogleSignIn([FromBody] GoogleSignInRequestModel request,
        CancellationToken cancellationToken)
    {
        var validUserResult = await _googleService.FindOrCreateGoogleUserAsync(request.Email,
            request.Name, request.ProfilePictureUrl, cancellationToken);

        var validUser = validUserResult.Payload;

        var accessToken = _jwtProvider.GenerateToken(validUser);

        var refreshToken = await _jwtProvider.CreateRefreshTokenAsync(validUser, cancellationToken);

        return new GoogleSignInResponseModel
        {
            Id = validUser.Id,
            Email = validUser.Email,
            Username = validUser.Username,
            ProfilePictureUrl = validUser.ProfilePictureUrl,
            Tokens = new TokensResponseModel(accessToken, refreshToken)
        };
    }

    [HttpPost("sign-out")]
    public async Task<Result<None>> Logout([FromBody] LogoutRequestModel request,
        CancellationToken cancellationToken)
    {
        var getUserIdResult = await _jwtProvider.GetUserIdByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!getUserIdResult.IsSucceed)
        {
            return getUserIdResult.ErrorCode;
        }

        var deleteRefreshTokensResult = await _jwtProvider
            .DeleteRefreshTokensResultAsync(getUserIdResult.Payload, cancellationToken);

        return deleteRefreshTokensResult.IsSucceed ? Result<None>.Success() : deleteRefreshTokensResult.ErrorCode;
    }
}