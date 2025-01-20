namespace Blog.Application.Configurations;

public class TranslatorConfiguration
{
    public static readonly string ConfigurationKey = "AzureTranslator";

    public string KeySectionName { get; init; }
    public string EndpointSectionName { get; init; }
    public string RegionSectionName { get; init; }
}