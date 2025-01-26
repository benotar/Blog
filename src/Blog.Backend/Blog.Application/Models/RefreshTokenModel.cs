namespace Blog.Application.Models;

public sealed record RefreshTokenModel
{
    public int Id { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset ExpiresOnUtc { get; set; }
    public UserModel User { get; set; }
}

