using Blog.API.Models.Request;
using Blog.API.Models.Response;
using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models;
using Blog.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

public class AuthController : BaseController
{
    private readonly IUserService _userService;
    private readonly IJwtProvider _jwtProvider;
    private readonly ICookieProvider _cookieProvider;

    public AuthController(IUserService userService, ICookieProvider cookieProvider, IJwtProvider jwtProvider)
    {
        _userService = userService;
        _cookieProvider = cookieProvider;
        _jwtProvider = jwtProvider;
    }

    [HttpPost("sign-up")]
    public async Task<Result<None>> SignUp([FromBody] SignUpRequestModel model, CancellationToken cancellationToken)
    {
        return await _userService
            .CreateAsync(model.Username, model.Email, model.Password, cancellationToken);
    }

    [HttpPost("sign-in")]

    // TODO Add dto
    public async Task<Result<SignInResponseModel>> Login([FromBody] SignInRequestModel model,
        CancellationToken cancellationToken)
    {
        var validUserResult = await _userService
            .GetCheckedUserAsync(model.Email, model.Password, cancellationToken);

        if (!validUserResult.IsSucceed)
        {
            return validUserResult.ErrorCode;
        }

        var validUser = validUserResult.Data;

        var accessTokenResult = _jwtProvider.GenerateToken(validUser.Id, validUser.Email, JwtType.Access);
        var refreshTokenResult = _jwtProvider.GenerateToken(validUser.Id, validUser.Email, JwtType.Refresh);

        if (!accessTokenResult.IsSucceed || !refreshTokenResult.IsSucceed)
        {
            return ErrorCode.JwtTokenIsUndefined;
        }

        var accessToken = accessTokenResult.Data;
        var refreshToken = refreshTokenResult.Data;

        _cookieProvider.AddTokens(HttpContext.Response, accessToken, refreshToken);

        return new SignInResponseModel
        {
            Id = validUser.Id,
            Email = validUser.Email,
            Username = validUser.Username,
        };
    }
}