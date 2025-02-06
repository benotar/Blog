using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Models.Response;

public class PagedList<T>
{
    private PagedList(List<T> items, int startIndex, int limit, int totalCount)
    {
        Items = items;
        StartIndex = startIndex;
        Limit = limit;
        TotalCount = totalCount;
    }

    public List<T> Items { get; }
    public int StartIndex { get; }
    public int Limit { get; }
    public int TotalCount { get; }
    
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int startIndex, int limit, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip(startIndex).Take(limit).ToListAsync(cancellationToken);

        return new PagedList<T>(items, startIndex, limit, totalCount);
    }
}