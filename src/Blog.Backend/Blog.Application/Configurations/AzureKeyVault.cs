namespace Blog.Application.Configurations;

public class AzureKeyVault
{
    public static readonly string ConfigurationKey = "AzureKeyVault";
    
    public string KeyVaultUrl { get; init; }
    public string ClientId { get; init; }
    public string DirectoryId { get; init; }
    public string ClientSecret { get; init; }
}