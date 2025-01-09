namespace Blog.Application.Configurations;

public class CookieConfiguration
{
    public static readonly string ConfigurationKey = "Cookie";
    
    public string AccessTokenKey { get; set; }
    public string RefreshTokenKey { get; set; }
}