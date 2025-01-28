namespace Blog.API.Models.Response.Auth;

public sealed record TokensResponseModel(string AccessToken, string RefreshToken);