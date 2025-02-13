using Blog.Application.Models.Response;

namespace Blog.Application.Interfaces.FactoryMethod;

public interface IPagination<TEntity>
{
    Task<PagedList<TEntity>> CreatePagedListAsync(IQueryable<TEntity> query, int? start, int? limit,
        CancellationToken cancellationToken = default);
}