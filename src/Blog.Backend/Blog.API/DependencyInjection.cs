using Azure.Identity;
using Blog.API.Extensions;
using Blog.API.Infrastructure;
using Blog.Application.Common;
using Blog.Application.Common.Converters;
using Blog.Application.Configurations;
using Blog.Domain.Enums;
using Blog.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.API;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add database configurations
        services.Configure<DatabaseConfiguration>(
            configuration.GetSection(DatabaseConfiguration.ConfigurationKey));

        services.Configure<JwtConfiguration>(
            configuration.GetSection(JwtConfiguration.ConfigurationKey));

        services.Configure<AzureKeyVault>(
            configuration.GetSection(AzureKeyVault.ConfigurationKey));

        services.Configure<TranslatorConfiguration>(
            configuration.GetSection(TranslatorConfiguration.ConfigurationKey));

        return services;
    }

    // Add configured controllers
    public static IServiceCollection AddControllersWithConfiguredApiBehavior(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters
                    .Add(new ServerResponseStringEnumConverter<ErrorCode>()))
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.GetErrors();

                    var details = new CustomValidationProblemDetails
                    {
                        Type = "https://httpstatuses.com/422",
                        Title = "Validation Error",
                        Detail = "One or more validation errors occurred",
                        Instance = context.HttpContext.Request.Path,
                        Errors = errors
                    };

                    var result = new UnprocessableEntityObjectResult(
                        new Result<CustomValidationProblemDetails>
                        {
                            ErrorCode = ErrorCode.InvalidModel,
                            Payload = details
                        });

                    result.ContentTypes.Add("application/json");

                    return result;
                };
            });

        return services;
    }

    // Add exception handler
    public static IServiceCollection AddExceptionHandlerWithProblemDetails(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        return services;
    }

    public static void AddConfiguredAzureKeyVault(this WebApplicationBuilder builder)
    {
        var azureServicesConfig = new AzureKeyVault();
        builder.Configuration.Bind(AzureKeyVault.ConfigurationKey, azureServicesConfig);

        var keyVaultUrl = new Uri(azureServicesConfig.KeyVaultUrl);
        var clientId = azureServicesConfig.ClientId;
        var clientSecret = azureServicesConfig.ClientSecret;
        var tenantId = azureServicesConfig.DirectoryId;

        //var azureCredential = new DefaultAzureCredential();

        var azureCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

        builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredential);
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}