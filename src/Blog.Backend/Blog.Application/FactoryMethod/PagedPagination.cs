using Blog.Application.Interfaces.FactoryMethod;
using Blog.Application.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.FactoryMethod;

public class PagedPagination<TEntity> : IPagination<TEntity>
{
    public async Task<PagedList<TEntity>> CreatePagedListAsync(IQueryable<TEntity> query, int? start, int? limit,
        CancellationToken cancellationToken = default)
    {
        var page = start is > 0 ? start.Value : 1;
        var pageSize = limit is > 0 ? limit.Value : 10;
        var totalCount = await query.CountAsync(cancellationToken);
        var hasNextPage = start * limit < totalCount;
        var hasPreviousPage = start > 1;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var paginationParameters = new PaginationParameters(
            page, 
            pageSize, 
            totalCount, 
            hasNextPage, 
            hasPreviousPage);

        return new PagedList<TEntity>(items, paginationParameters);
    }
}