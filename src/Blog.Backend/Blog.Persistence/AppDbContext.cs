using Blog.Application.Interfaces.DbContext;
using Blog.Domain.Entities;
using Blog.Persistence.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Blog.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IDbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}