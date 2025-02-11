using System.Security.Claims;
using System.Text;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IEncryptionProvider, HmacSha256Provider>();
        services.AddSingleton<IMomentProvider, MomentProvider>();

        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IGoogleService, GoogleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAzureTranslatorService, AzureTranslatorService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();

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

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = new JwtConfiguration();

        configuration.Bind(JwtConfiguration.ConfigurationKey, jwtConfig);

        var secretKey = configuration.GetSection(jwtConfig.KeySectionName).Value;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("admin", policy => policy.RequireRole("Admin"))
            .AddPolicy("user", policy => policy.RequireRole("User"));

        return services;
    }

    public static IServiceCollection AddTextTranslator(this IServiceCollection services, IConfiguration configuration)
    {
        var translatorConfig = new TranslatorConfiguration();
        configuration.Bind(TranslatorConfiguration.ConfigurationKey, translatorConfig);

        var key = configuration.GetSection(translatorConfig.KeySectionName).Value;
        var endpoint = configuration.GetSection(translatorConfig.EndpointSectionName).Value;
        var region = configuration.GetSection(translatorConfig.RegionSectionName).Value;
        var credential = new AzureKeyCredential(key);
        var endpointUrl = new Uri(endpoint);

        var client = new TextTranslationClient(credential, endpointUrl, region);

        services.AddSingleton(client);

        return services;
    }
}