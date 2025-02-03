using Blog.Persistence.GenericRepository;
using Blog.Application.Configurations;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Domain.Enums;
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

            options.UseNpgsql(connectionString,
                dbContextOptionsBuilder => dbContextOptionsBuilder.MapEnum<PostCategory>("category"));
        });
        
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}