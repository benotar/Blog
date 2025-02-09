using System.Text.Json.Serialization;
using Blog.Domain.Enums;

namespace Blog.Domain.Entities;

public class Post : DbEntity
{
    public int UserId { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public PostCategory Category { get; set; }
    public string Slug { get; set; }

    // EF Navigation Property
    [JsonIgnore] public User? User { get; init; }
}