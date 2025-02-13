using System.Linq.Expressions;

namespace Blog.Application.Interfaces.Repository;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);

    Task<int> RemoveAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    IQueryable<T> AsNoTracking();

    bool IsModified(T entity);
}