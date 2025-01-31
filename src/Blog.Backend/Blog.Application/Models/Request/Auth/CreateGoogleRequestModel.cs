namespace Blog.Application.Models.Request.Auth;

public record CreateGoogleRequestModel
{
    public string Username { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string PictureUrl { get; init; }
}