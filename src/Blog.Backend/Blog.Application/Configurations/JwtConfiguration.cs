namespace Blog.Application.Configurations;

public class JwtConfiguration
{
    public static readonly string ConfigurationKey = "JWT";
    
    public string KeySectionName { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public int AccessExpirationMinutes { get; init; }
    public int RefreshExpirationDays { get; init; }
}