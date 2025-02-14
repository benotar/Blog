using Blog.Application.Common;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;

namespace Blog.Application.Interfaces.Services;

public interface ICommentService
{
    Task<Result<CommentModel>> CreateAsync(int userId, CreateCommentRequestModel request,
        CancellationToken cancellationToken = default);

    Task<Result<PagedList<CommentModel>>> GetAsync(int postId, GetCommentsOfPostRequestModel request,
        CancellationToken cancellationToken = default);

    Task<Result<CommentModel>> LikeAsync(int userId, int commentId, CancellationToken cancellationToken = default);
    Task<Result<CommentModel>> UpdateAsync(int commentId, (int userId, string userRole) userData, UpdateCommentRequestModel request, 
        CancellationToken cancellationToken = default);
}