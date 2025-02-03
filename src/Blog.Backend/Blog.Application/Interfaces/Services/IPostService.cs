using Blog.Application.Common;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Services;

public interface IPostService
{
    Task<Result<PostModel>> CreateAsync(int userId, CreatePostRequest request, CancellationToken cancellationToken = default);
    Result<IEnumerable<PostCategory>> GetCategories();
}