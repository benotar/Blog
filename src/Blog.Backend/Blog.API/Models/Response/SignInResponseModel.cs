namespace Blog.API.Models.Response;

public sealed record SignInResponseModel
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string ProfilePictureUrl { get; init; }
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
}