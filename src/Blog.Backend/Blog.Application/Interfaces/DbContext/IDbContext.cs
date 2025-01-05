namespace Blog.Application.Interfaces.DbContext;


public interface IDbContext : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}