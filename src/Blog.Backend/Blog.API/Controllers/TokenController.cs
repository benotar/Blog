using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Response.Auth;
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
        var verifyAndGetTokenResult = await _jwtProvider
            .VerifyAndGetRefreshTokenAsync(request.RefreshToken, request.UserId, cancellationToken);

        if (!verifyAndGetTokenResult.IsSucceed)
        {
            return verifyAndGetTokenResult.ErrorCode;
        }

        var targetRefreshToken = verifyAndGetTokenResult.Payload;
        var accessToken = _jwtProvider.GenerateToken(targetRefreshToken.User);
        
        var updateTargetRefreshTokenResult = await _jwtProvider.UpdateRefreshTokenAsync(targetRefreshToken.Id, targetRefreshToken.Token,
            targetRefreshToken.User, cancellationToken);

        if (!updateTargetRefreshTokenResult.IsSucceed)
        {
            return updateTargetRefreshTokenResult.ErrorCode;
        }

        return new TokensResponseModel(accessToken, updateTargetRefreshTokenResult.Payload);
    }
}