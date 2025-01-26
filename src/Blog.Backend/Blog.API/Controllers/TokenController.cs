using Blog.API.Models.Request;
using Blog.API.Models.Response;
using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

public class TokenController : BaseController
{
    private readonly IJwtProvider _jwtProvider;

    public TokenController(IJwtProvider jwtProvider)
    {
        _jwtProvider = jwtProvider;
    }

    [HttpPost("refresh")]
    public async Task<Result<TokensResponseModel>> Refresh([FromBody] RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var refreshTokenResult = await _jwtProvider
            .VerifyAndGetRefreshTokenAsync(request.RefreshToken, request.UserId, cancellationToken);

        if (!refreshTokenResult.IsSucceed)
        {
            return refreshTokenResult.ErrorCode;
        }

        var targetRefreshToken = refreshTokenResult.Payload;
        var accessToken = _jwtProvider.GenerateToken(targetRefreshToken.User);
        var updateResult = await _jwtProvider.UpdateRefreshTokenAsync(targetRefreshToken.Token,
            targetRefreshToken.User, cancellationToken);

        if (!updateResult.IsSucceed)
        {
            return updateResult.ErrorCode;
        }

        return new TokensResponseModel(accessToken, updateResult.Payload);
    }
}