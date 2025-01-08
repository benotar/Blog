using System.Reflection;
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

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add database configurations
        services.Configure<DatabaseConfiguration>(
            configuration.GetSection(DatabaseConfiguration.ConfigurationKey));

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
}