using Blog.Application.Models;
using Blog.Domain.Entities;

namespace Blog.Application.Extensions;

public static class ModelExtensions
{
    public static UserModel ToModel(this User user)
    {
        return new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
        };
    }

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