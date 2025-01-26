using Blog.Application.Common;
using Blog.Application.Interfaces.DbContext;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Models;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence.Repository;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbContext _context;

    public RefreshTokenRepository(IDbContext context)
    {
        _context = context;
    }

    public void Add(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenIncludeUserAsync(string newRefreshToken, int userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Token == newRefreshToken,
                cancellationToken);
    }

    public async Task<Result<None>> UpdateAsync(string targetToken, string newToken, DateTimeOffset newExpireOnUtc,
        CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _context.RefreshTokens
            .Where(r => r.Token == targetToken)
            .ExecuteUpdateAsync(updates => updates
                    .SetProperty(rf => rf.Token, newToken)
                    .SetProperty(rf => rf.ExpiresOnUtc, newExpireOnUtc),
                cancellationToken
            );

        return rowsAffected == 0 ? ErrorCode.InvalidRefreshToken : Result<None>.Success();
    }
}