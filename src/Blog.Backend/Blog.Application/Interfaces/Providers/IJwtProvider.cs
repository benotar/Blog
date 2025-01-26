using Blog.Application.Common;
using Blog.Application.Models;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Providers;

public interface IJwtProvider
{
    Result<string> GenerateToken(int userId, string email, JwtType jwtType);
    string GenerateToken(UserModel user);
    Task<string> CreateRefreshTokenAsync(UserModel user, CancellationToken cancellationToken = default);
    
    Task<Result<RefreshTokenModel?>> VerifyAndGetRefreshTokenAsync(string refreshToken, 
        CancellationToken cancellationToken = default);
    Task<Result<string>> UpdateRefreshTokenAsync(string targetToken, UserModel user, CancellationToken cancellationToken = default);
}