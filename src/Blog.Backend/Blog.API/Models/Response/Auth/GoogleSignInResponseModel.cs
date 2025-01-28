namespace Blog.API.Models.Response.Auth;

public sealed record GoogleSignInResponseModel
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string ProfilePictureUrl { get; init; }
    public TokensResponseModel Tokens { get; init; }
}