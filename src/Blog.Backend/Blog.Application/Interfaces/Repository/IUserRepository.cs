using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> AnyByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);

    Task<int> UpdateAsync(int userId, string username, string email, string profilePictureUrl,
        CancellationToken cancellationToken = default);
}