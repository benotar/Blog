using Blog.Application.Interfaces.DbContext;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.UnitOfWork;

namespace Blog.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository UserRepository { get; }
    public IRefreshTokenRepository RefreshTokenRepository { get; }

    private readonly IDbContext _dbContext;
    private bool _disposed = false;

    public UnitOfWork(IDbContext dbContext, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
    {
        _dbContext = dbContext;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}