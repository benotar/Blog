using Blog.Application.Common;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;

namespace Blog.Application.Interfaces.Services;

public interface ICommentService
{
    Task<Result<CommentModel>> CreateAsync(int userId, CreateCommentRequestModel request, CancellationToken cancellationToken);
}