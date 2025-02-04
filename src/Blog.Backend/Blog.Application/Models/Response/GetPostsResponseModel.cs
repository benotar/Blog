namespace Blog.Application.Models.Response;

public sealed record GetPostsResponseModel
{
    public PagedList<PostModel> Items { get; init; }
    public int LastMonthPostsCount { get; init; }
}