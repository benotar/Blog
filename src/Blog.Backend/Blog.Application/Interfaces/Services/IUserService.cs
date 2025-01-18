using Blog.Application.Common;
using Blog.Application.Models;
using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default);
    Task<Result<UserModel>> CreateGoogleAsync(string name, string email, string pictureUrl,
        CancellationToken cancellationToken = default);
    Task<Result<UserModel>> GetCheckedUserAsync(string email, string password,
        CancellationToken cancellationToken = default);
    
}