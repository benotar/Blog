using System.Linq.Expressions;
using Blog.Application.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence.GenericRepository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken: cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await _dbSet.AnyAsync(cancellationToken: cancellationToken)
            : await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await _dbSet.FirstOrDefaultAsync(cancellationToken: cancellationToken)
            : await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }
    
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken: cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public async Task<int> RemoveAsync(Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        return await query.ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }

    public IQueryable<T> AsQueryable()
    {
        return _dbSet.AsNoTracking();
    }
}