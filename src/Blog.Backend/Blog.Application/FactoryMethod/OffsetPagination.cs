using Blog.Application.Interfaces.FactoryMethod;
using Blog.Application.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.FactoryMethod;

public class OffsetPagination<TEntity> : IPagination<TEntity>
{
    public async Task<PagedList<TEntity>> CreatePagedListAsync(IQueryable<TEntity> query, int? start, int? limit,
        CancellationToken cancellationToken = default)
    {
        var startIndex = start is >= 0 ? start.Value : 0;
        var itemsSize = limit is > 0 ? limit.Value : 9;
        var totalCount = await query.CountAsync(cancellationToken);
        
        var hasNextPage = (startIndex + 1) * itemsSize < totalCount;
        var hasPreviousPage = startIndex > 0;

        var items = await query
            .Skip(startIndex)
            .Take(itemsSize)
            .ToListAsync(cancellationToken);

        var paginationParameters = new PaginationParameters(
            startIndex, 
            itemsSize, 
            totalCount, 
            hasNextPage, 
            hasPreviousPage);

        return new PagedList<TEntity>(items, paginationParameters);
    }
}