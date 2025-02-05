namespace Blog.Application.Models.Response;

public sealed record GetPostsResponseModel
{
    public PagedList<PostModel> Data { get; init; }
    public int LastMonthPostsCount { get; init; }
}