using Blog.Domain.Entities;

namespace Blog.Application.Models.Response;

public class LikeModel
{
    public int UserId { get; init; }
}

public static class LikeModelExtensions
{
    public static LikeModel ToModel(this Like like)
    {
        return new LikeModel
        {
            UserId = like.UserId
        };
    }
}