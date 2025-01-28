using Blog.API.Models.Response.User;
using Blog.Application.Models;

namespace Blog.API.Extensions;

public static class ModelExtensions
{
    public static UserResponseModel ToModel(this UserModel user)
    {
        return new UserResponseModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }
}