using System.Text.Json.Serialization;

namespace Blog.Domain.Entities;

public class Like
{
    public int CommentId { get; set; }
    public int UserId { get; set; }
    
    [JsonIgnore]public Comment? Comment { get; init; }
}