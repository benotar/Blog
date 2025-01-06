using System.Text.Json;
using Blog.Application.Common.Converters;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Providers;
using Blog.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IEncryptionProvider, HmacSha256Provider>();
        services.AddSingleton<IMomentProvider, MomentProvider>();

        services.AddScoped<IUserService, IUserService>();
        
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
}