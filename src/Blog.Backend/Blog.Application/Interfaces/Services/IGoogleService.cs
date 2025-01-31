using Blog.Application.Common;
using Blog.Application.Models;
using Blog.Application.Models.Response.User;

namespace Blog.Application.Interfaces.Services;

public interface IGoogleService
{
    Task<Result<UserModel>> FindOrCreateGoogleUserAsync(string email, string name, string pictureUrl,
        CancellationToken cancellationToken = default);
}