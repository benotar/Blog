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
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var verifyResult = await _jwtProvider
            .VerifyAndGetRefreshTokenAsync(request.RefreshToken, request.UserId, cancellationToken);

        if (!verifyResult.IsSucceed)
            return ToActionResult(verifyResult);

        var targetRefreshToken = verifyResult.Payload;
        var accessToken = _jwtProvider.GenerateToken(targetRefreshToken.User);

        var updateResult = await _jwtProvider.UpdateRefreshTokenAsync(targetRefreshToken.Id,
            targetRefreshToken.Token, targetRefreshToken.User, cancellationToken);

        if (!updateResult.IsSucceed)
            return ToActionResult(updateResult);

        return ToActionResult(Result<TokensResponseModel>.Success(
            new TokensResponseModel(accessToken, updateResult.Payload)));
    }
}
