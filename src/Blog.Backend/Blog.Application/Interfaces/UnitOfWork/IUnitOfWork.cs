using Blog.Application.Interfaces.Repository;

namespace Blog.Application.Interfaces.UnitOfWork;

public interface IUnitOfWork: IDisposable
{
    IUserRepository UserRepository { get; }
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}