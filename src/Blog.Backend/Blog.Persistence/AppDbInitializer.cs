using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blog.Persistence;

public static class AppDbInitializer
{
    public static async Task ApplyMigrationsAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No migrations applied.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while initializing the database.");
            throw;
        }
    }
}