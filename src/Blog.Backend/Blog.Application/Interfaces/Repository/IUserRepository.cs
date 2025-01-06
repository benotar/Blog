using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<bool> AnyByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
}