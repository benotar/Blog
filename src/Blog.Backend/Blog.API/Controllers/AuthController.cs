using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Response.Auth;
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
    public async Task<IActionResult> SignUp([FromBody] SignUpRequestModel model, CancellationToken cancellationToken)
    {
        var result = await _userService
            .CreateAsync(model.Username, model.Email, model.Password, cancellationToken);

        return ToActionResult(result);
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> Login([FromBody] SignInRequestModel model,
        CancellationToken cancellationToken)
    {
        var validUserResult = await _userService
            .GetCheckedUserAsync(model.Email, model.Password, cancellationToken);

        if (!validUserResult.IsSucceed)
        {
            return ToActionResult(validUserResult);
        }

        var validUser = validUserResult.Payload;
        var accessToken = _jwtProvider.GenerateToken(validUser);
        var refreshToken = await _jwtProvider.CreateRefreshTokenAsync(validUser, cancellationToken);

        return ToActionResult(Result<SignInResponseModel>.Success(new SignInResponseModel
        {
            CurrentUser = validUser, Tokens = new TokensResponseModel(accessToken, refreshToken)
        }));
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequestModel request,
        CancellationToken cancellationToken)
    {
        var validUserResult = await _googleService.FindOrCreateGoogleUserAsync(request.Email,
            request.Name, request.ProfilePictureUrl, cancellationToken);

        var validUser = validUserResult.Payload;
        var accessToken = _jwtProvider.GenerateToken(validUser);
        var refreshToken = await _jwtProvider.CreateRefreshTokenAsync(validUser, cancellationToken);

        return ToActionResult(Result<SignInResponseModel>.Success(new SignInResponseModel
        {
            CurrentUser = validUser, Tokens = new TokensResponseModel(accessToken, refreshToken)
        }));
    }

    [HttpPost("sign-out")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestModel request,
        CancellationToken cancellationToken)
    {
        var getUserIdResult = await _jwtProvider.GetUserIdByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!getUserIdResult.IsSucceed)
        {
            return ToActionResult(getUserIdResult);
        }

        var deleteRefreshTokensResult = await _jwtProvider
            .DeleteRefreshTokensResultAsync(getUserIdResult.Payload, cancellationToken);

        return deleteRefreshTokensResult.IsSucceed
            ? ToActionResult(Result<None>.Success())
            : ToActionResult(deleteRefreshTokensResult);

    }
}
