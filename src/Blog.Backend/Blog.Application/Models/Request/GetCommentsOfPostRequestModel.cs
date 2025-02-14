namespace Blog.Application.Models.Request;

public sealed record GetCommentsOfPostRequestModel
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}

public sealed record GetCommentsRequestModel
{
    public int? StartIndex { get; init; }
    public int? Limit { get; init; }
    public string? OrderBy { get; init; }
}