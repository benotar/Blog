namespace Blog.API.Models.Response;

public record GoogleSignInResponseModel
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string ProfilePictureUrl { get; init; }
}