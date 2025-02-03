using System.Text.Json.Serialization;

namespace Blog.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset ExpiresOnUtc { get; set; }

    // EF Navigation Property
    [JsonIgnore] public User? User { get; set; }
}