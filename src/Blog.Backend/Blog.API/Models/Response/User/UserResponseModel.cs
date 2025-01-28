namespace Blog.API.Models.Response.User;

public sealed record UserResponseModel
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string ProfilePictureUrl { get; init; }
}