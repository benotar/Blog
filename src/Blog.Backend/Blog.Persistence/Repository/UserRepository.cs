using Blog.Application.Interfaces.DbContext;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence.Repository;

public class UserRepository : IUserRepository
{
    private readonly IDbContext _dbContext;
    private readonly IMomentProvider _momentProvider;

    public UserRepository(IDbContext dbContext, IMomentProvider momentProvider)
    {
        _dbContext = dbContext;
        _momentProvider = momentProvider;
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AnyByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }

    public async Task<int> DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await  _dbContext.Users.Where(u => u.Id == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}