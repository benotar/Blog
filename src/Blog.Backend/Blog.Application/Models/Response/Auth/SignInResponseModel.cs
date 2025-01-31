using Blog.Application.Models.Response.User;

namespace Blog.Application.Models.Response.Auth;

public sealed record SignInResponseModel
{
    public UserModel CurrentUser { get; init; }
    public TokensResponseModel Tokens { get; init; }
}