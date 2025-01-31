using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;

namespace Blog.Application.Models.Response.Auth;

public sealed record RefreshTokenModel
{
    public int Id { get; set; }
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset ExpiresOnUtc { get; set; }
    public UserModel User { get; set; }
}

public static class RefreshTokenModelExtensions
{
    public static RefreshTokenModel ToModel(this RefreshToken refreshToken)
    {
        return new RefreshTokenModel
        {
            Id = refreshToken.Id,
            Token = refreshToken.Token,
            UserId = refreshToken.UserId,
            ExpiresOnUtc = refreshToken.ExpiresOnUtc,
            User = refreshToken.User?.ToModel()
        };
    }
}