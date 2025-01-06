using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Interfaces.DbContext;


public interface IDbContext : IDisposable
{
    DbSet<User> Users { get; set; } 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}