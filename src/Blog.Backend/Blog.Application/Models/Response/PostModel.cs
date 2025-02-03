using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Models.Response;

public record PostModel
{
    public int Id { get; init; }
    public string Content { get; init; }
    public string Title { get; init; }
    public string ImageUrl { get; init; }
    public PostCategory Category { get; init; }
    public string Slug { get; init; }
};

public static class PostModelExtensions
{
    public static PostModel ToModel(this Post post)
    {
        return new PostModel
        {
            Id = post.Id,
            Content = post.Content,
            Title = post.Title,
            ImageUrl = post.ImageUrl,
            Category = post.Category,
            Slug = post.Slug,
        };
    }
}