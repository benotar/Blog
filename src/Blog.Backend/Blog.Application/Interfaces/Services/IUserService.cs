using Blog.Application.Common;

namespace Blog.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default);
}