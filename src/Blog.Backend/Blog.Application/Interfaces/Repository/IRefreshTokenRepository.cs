using Blog.Application.Common;
using Blog.Application.Models;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);

    Task<RefreshToken?> GetRefreshTokenIncludeUserAsync(string newRefreshToken, int userId,
        CancellationToken cancellationToken = default);

    Task<Result<None>> UpdateAsync(int tokenId, string targetToken, string newToken, DateTimeOffset newExpire,
        CancellationToken cancellationToken = default);
}