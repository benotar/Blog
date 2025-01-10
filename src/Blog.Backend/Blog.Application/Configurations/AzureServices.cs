namespace Blog.Application.Configurations;

public class AzureServices
{
    public static readonly string ConfigurationKey = "AzureServices";
    
    public string KeyVaultUrl { get; init; }
    public string ClientId { get; init; }
    public string DirectoryId { get; init; }
    public string ClientSecret { get; init; }

}