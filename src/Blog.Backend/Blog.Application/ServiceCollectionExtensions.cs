using System.Text.Json;
using Azure;
using Azure.AI.Translation.Text;
using Blog.Application.Common.Converters;
using Blog.Application.Configurations;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Providers;
using Blog.Application.Services;
using Blog.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IEncryptionProvider, HmacSha256Provider>();
        services.AddSingleton<IMomentProvider, MomentProvider>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddSingleton<ICookieProvider, CookieProvider>();

        services.AddScoped<IGoogleService, GoogleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAzureTranslatorService, AzureTranslatorService>();

        
        // Add JsonSerializerOptions 
        var jsonOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new ServerResponseStringEnumConverter<ErrorCode>()
            },
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };

        services.AddSingleton(jsonOptions);

        return services;
    }

    public static IServiceCollection AddTextTranslator(this IServiceCollection services, IConfiguration configuration)
    {
        var translatorConfig = new TranslatorConfiguration();
        configuration.Bind(TranslatorConfiguration.ConfigurationKey, translatorConfig);

        var key = configuration.GetSection( translatorConfig.Key).Value;
        var endpoint = configuration.GetSection(translatorConfig.Endpoint).Value;
        var region = configuration.GetSection(translatorConfig.Region).Value;
        var credential = new AzureKeyCredential(key);
        var endpointUrl = new Uri(endpoint);

        var client = new TextTranslationClient(credential, endpointUrl, region);

        services.AddSingleton(client);
        
        return services;
    }
}