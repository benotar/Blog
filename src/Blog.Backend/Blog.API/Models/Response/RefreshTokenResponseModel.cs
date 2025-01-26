namespace Blog.API.Models.Response;

public sealed record RefreshTokenResponseModel(string AccessToken, string RefreshToken);