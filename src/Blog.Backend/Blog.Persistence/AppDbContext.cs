using Blog.Application.Interfaces.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}