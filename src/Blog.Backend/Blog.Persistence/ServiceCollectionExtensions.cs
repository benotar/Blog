using System.Reflection;
using Blog.Application.Configurations;
using Blog.Application.Interfaces.DbContext;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConfig = new DatabaseConfiguration();

        configuration.Bind(DatabaseConfiguration.ConfigurationKey, dbConfig);

        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = string.Format(dbConfig.ConnectionString, dbConfig.User, dbConfig.Password,
                dbConfig.Host, dbConfig.DbName);

            options.UseNpgsql(connectionString);

            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IDbContext>(provider =>
            provider.GetRequiredService<AppDbContext>());


        services.AddScoped<IUserRepository, UserRepository>();

        // Add UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        

        return services;
    }
}