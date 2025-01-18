namespace Blog.Application.Configurations;

public class TranslatorConfiguration
{
    public static readonly string ConfigurationKey = "AzureTranslator";

    public string Key { get; init; }
    public string Endpoint { get; init; }
    public string Region { get; init; }
}