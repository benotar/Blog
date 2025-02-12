using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;

namespace Blog.Application.Models.Response;

public sealed record CommentModel
{
    public int Id { get; init; }
    public string Content { get; init; }
    public int PostId { get; init; }
    public UserModel Author { get; init; }
    public IEnumerable<LikeModel> Likes { get; init; }
    public int CountOfLikes { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public static class CommentModelExtensions
{
    public static CommentModel ToModel(this Comment comment)
    {
        return new CommentModel
        {
            Id = comment.Id,
            Content = comment.Content,
            PostId = comment.PostId,
            Author = comment.Author?.ToModel(),
            Likes = comment.Likes.Select(like => like.ToModel()),
            CountOfLikes = comment.CountOfLikes,
            CreatedAt = comment.CreatedAt
        };
    }
}