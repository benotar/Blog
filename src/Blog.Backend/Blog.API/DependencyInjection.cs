using Azure.Identity;
using Blog.API.Extensions;
using Blog.API.Infrastructure;
using Blog.Application.Common;
using Blog.Application.Common.Converters;
using Blog.Application.Configurations;
using Blog.Application.Models.Abstraction;
using Blog.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add database configurations
        services.Configure<DatabaseConfiguration>(
            configuration.GetSection(DatabaseConfiguration.ConfigurationKey));
        
        services.Configure<CookieConfiguration>(
            configuration.GetSection(CookieConfiguration.ConfigurationKey));
        
        services.Configure<JwtConfiguration>(
            configuration.GetSection(JwtConfiguration.ConfigurationKey));
        
        services.Configure<AzureServices>(
            configuration.GetSection(AzureServices.ConfigurationKey));

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
                            Data = details
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

    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        var applicationAssembly = typeof(BaseModel<,>).Assembly;
        typeAdapterConfig.Scan(applicationAssembly);

        return services;
    }
    public static void AddConfiguredAzureKeyVault(this WebApplicationBuilder builder)
    {
        var azureServicesConfig = new AzureServices();
        builder.Configuration.Bind(AzureServices.ConfigurationKey, azureServicesConfig);

        var keyVaultUrl = new Uri(azureServicesConfig.KeyVaultUrl);
        var clientId = azureServicesConfig.ClientId;
        var clientSecret = azureServicesConfig.ClientSecret;
        var tenantId = azureServicesConfig.DirectoryId;
        
        //var azureCredential = new DefaultAzureCredential();

        var azureCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        
        builder.Configuration.AddAzureKeyVault(keyVaultUrl, azureCredential);
    }
    
}