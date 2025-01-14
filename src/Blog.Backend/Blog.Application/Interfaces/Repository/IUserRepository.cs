using Blog.Application.Models;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> AnyByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
}