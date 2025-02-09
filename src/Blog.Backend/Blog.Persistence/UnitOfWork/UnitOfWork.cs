using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Persistence.GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Persistence.UnitOfWork;

public class UnitOfWork(AppDbContext context, IServiceProvider serviceProvider)
    : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = new();
    private readonly DbContext _context = context;

    public IRepository<T> GetRepository<T>() where T : class
    {
        if (!_repositories.ContainsKey(typeof(T)))
        {
            _repositories[typeof(T)] = new Repository<T>(_context);
        }

        return (IRepository<T>)_repositories[typeof(T)];
    }

    public TRepository GetEntityRepository<TRepository>() where TRepository : class
    {
        return serviceProvider.GetRequiredService<TRepository>();
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}