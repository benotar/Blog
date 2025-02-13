namespace Blog.Application.Models.Response;

public sealed record PaginationParameters(int Start, int Limit, int TotalCount, bool HasNextPage, bool HasPreviousPage);

public sealed record PagedList<T>
{
    public List<T> Items { get; }
    public int Start { get; }
    public int Limit { get; }
    public int TotalCount { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }

    public PagedList(List<T> items, PaginationParameters paginationParameters)
    {
        Items = items;
        Start = paginationParameters.Start;
        Limit = paginationParameters.Limit;
        TotalCount = paginationParameters.TotalCount;
        HasNextPage = paginationParameters.HasNextPage;
        HasPreviousPage = paginationParameters.HasPreviousPage;
    }
    private PagedList() { }
}