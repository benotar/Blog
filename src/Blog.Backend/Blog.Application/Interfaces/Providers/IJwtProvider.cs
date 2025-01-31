using Blog.Application.Common;
using Blog.Application.Models;
using Blog.Application.Models.Response.Auth;
using Blog.Application.Models.Response.User;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Providers;

public interface IJwtProvider
{
    string GenerateToken(UserModel user);
    Task<string> CreateRefreshTokenAsync(UserModel user, CancellationToken cancellationToken = default);

    Task<Result<RefreshTokenModel?>> VerifyAndGetRefreshTokenAsync(string refreshToken, int userId,
        CancellationToken cancellationToken = default);

    Task<Result<RefreshTokenModel>?> GetRefreshTokenEntityAsync(string refreshToken, int userId,
        CancellationToken cancellationToken = default);

    Task<Result<None>> DeleteRefreshTokensResultAsync(int userId, CancellationToken cancellationToken = default);

    Task<Result<string>> UpdateRefreshTokenAsync(int targetTokenId, string targetToken, UserModel user,
        CancellationToken cancellationToken = default);

    Task<Result<int>> GetUserIdByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}