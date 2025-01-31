namespace Blog.Application.Models.Response.User;

public record UserModel
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string ProfilePictureUrl { get; init; }
}

public static class UserModelExtensions
{
    public static UserModel ToModel(this Domain.Entities.User user)
    {
        return new UserModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
        };
    }
    
}