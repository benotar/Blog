namespace Blog.Application.Models.Response.Auth;

public sealed record TokensResponseModel(string AccessToken, string RefreshToken);