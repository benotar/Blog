using Blog.Application.Common;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);

    Task<RefreshToken?> GetIncludeUserAsync(string refreshToken, int userId,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> Get(string refreshToken, int userId,
        CancellationToken cancellationToken = default);

    Task<Result<None>> UpdateAsync(int tokenId, string targetToken, string newToken, DateTimeOffset newExpire,
        CancellationToken cancellationToken = default);

    Task<Result<None>> DeleteAllAsync(int userId, CancellationToken cancellationToken = default);
    Task<int> GetUserIdByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}