using System.Text.Json.Serialization;
using Blog.Domain.Enums;

namespace Blog.Domain.Entities;

public class Post : DbEntity
{
    public int UserId { get; init; }
    public string Content { get; init; }
    public string Title { get; init; }
    public string ImageUrl { get; set; }
    public PostCategory Category { get; init; }
    public string Slug { get; init; }

    // EF Navigation Property
    [JsonIgnore] public User? User { get; init; }
}