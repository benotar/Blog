using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> AnyByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
    Task<int> DeleteAsync(int userId, CancellationToken cancellationToken = default);
}