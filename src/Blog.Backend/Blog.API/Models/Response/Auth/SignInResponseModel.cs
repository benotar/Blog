using Blog.API.Models.Response.User;

namespace Blog.API.Models.Response.Auth;

public sealed record SignInResponseModel
{
    public UserResponseModel CurrentUser { get; init; }
    public TokensResponseModel Tokens { get; init; }
}