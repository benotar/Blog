using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services;

public class CommentService : ICommentService
{
    private readonly IMomentProvider _momentProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Comment> _commentRepository;

    public CommentService(IMomentProvider momentProvider, IUnitOfWork unitOfWork)
    {
        _momentProvider = momentProvider;
        _unitOfWork = unitOfWork;
        _commentRepository = _unitOfWork.GetRepository<Comment>();
    }

    public async Task<Result<CommentModel>> CreateAsync(int userId, CreateCommentRequestModel request,
        CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.GetRepository<Post>()
                .AnyAsync(post => post.Id == request.PostId, cancellationToken))
        {
            return ErrorCode.PostNotFound;
        }

        if (!await _unitOfWork.GetRepository<User>()
                .AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return ErrorCode.UserNotFound;
        }

        var newComment = new Comment
        {
            Content = request.Content,
            AuthorId = userId,
            PostId = request.PostId,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        await _commentRepository.AddAsync(newComment, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newComment.ToModel();
    }
}