using Blog.Application.Configurations;

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
}