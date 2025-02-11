using System.Text.Json.Serialization;

namespace Blog.Domain.Entities;

public class Comment : DbEntity
{
    public string Content { get; set; }
    public int PostId { get; set; }
    public int AuthorId { get; set; }
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public int CountOfLikes => Likes.Count;
    
    [JsonIgnore]public User? Author { get; init; }
    [JsonIgnore]public Post? Post { get; init; }
}