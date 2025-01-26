using Blog.Application.Common;
using Blog.Application.Models;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenIncludeUserAsync(string newRefreshToken, CancellationToken cancellationToken = default);
    Task<Result<None>> UpdateAsync(string targetToken, string newToken, DateTimeOffset newExpireOnUtc,
        CancellationToken cancellationToken = default);
}