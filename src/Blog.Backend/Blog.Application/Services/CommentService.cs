using System.Linq.Expressions;
using Blog.Application.Common;
using Blog.Application.Interfaces.FactoryMethod;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Application.Models.Response.User;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Services;

public class CommentService : ICommentService
{
    private readonly IMomentProvider _momentProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Comment> _commentRepository;
    private readonly IPaginationFactory<CommentModel> _commentPaginationFactory;

    public CommentService(IMomentProvider momentProvider, IUnitOfWork unitOfWork,
        IPaginationFactory<CommentModel> commentPaginationFactory)
    {
        _momentProvider = momentProvider;
        _unitOfWork = unitOfWork;
        _commentPaginationFactory = commentPaginationFactory;
        _commentRepository = _unitOfWork.GetRepository<Comment>();
    }

    public async Task<Result<CommentModel>> CreateAsync(int userId, CreateCommentRequestModel request,
        CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.GetRepository<Post>()
                .AnyAsync(post => post.Id == request.PostId, cancellationToken))
        {
            return ErrorCode.PostNotFound;
        }

        var existingUser = await _unitOfWork.GetRepository<User>().GetByIdAsync(userId, cancellationToken);

        if (existingUser is null)
        {
            return ErrorCode.UserNotFound;
        }

        var newComment = new Comment
        {
            Content = request.Content,
            AuthorId = userId,
            Author = existingUser,
            PostId = request.PostId,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        await _commentRepository.AddAsync(newComment, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newComment.ToModel();
    }

    public async Task<Result<PagedList<CommentModel>>> GetAsync(int postId, GetCommentsOfPostRequestModel request,
        CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.GetRepository<Post>().AnyAsync(post => post.Id == postId, cancellationToken))
        {
            return ErrorCode.PostNotFound;
        }

        var commentsQuery = _commentRepository
            .AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .Include(comment => comment.Likes)
            .OrderByDescending(comment => comment.CreatedAt);

        var commentsModels = commentsQuery
            .Select(comment => new CommentModel
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                Author = new UserModel
                {
                    Id = comment.Author.Id,
                    Username = comment.Author.Username,
                    Email = comment.Author.Email,
                    ProfilePictureUrl = comment.Author.ProfilePictureUrl,
                    Role = comment.Author.Role,
                    CreatedAt = comment.Author.CreatedAt
                },
                Likes = comment.Likes
                    .Select(like => new LikeModel
                    {
                        UserId = like.UserId
                    }),
                CountOfLikes = comment.CountOfLikes,
                CreatedAt = comment.CreatedAt
            });

        return await _commentPaginationFactory
            .CreatePaginationFactory(PaginationType.Paged)
            .CreatePagedListAsync(
                commentsModels,
                request.Page,
                request.PageSize,
                cancellationToken);
    }

    public async Task<Result<CommentModel>> LikeAsync(int userId, int commentId,
        CancellationToken cancellationToken = default)
    {
        var existingComment = await _commentRepository
            .AsQueryable()
            .Include(comment => comment.Author)
            .Include(comment => comment.Likes)
            .FirstOrDefaultAsync(comment => comment.Id == commentId, cancellationToken);

        if (existingComment is null)
        {
            return ErrorCode.CommentNotFound;
        }

        var existingLike =
            existingComment.Likes.FirstOrDefault(like => like.CommentId == commentId && like.UserId == userId);

        if (existingLike is not null)
        {
            existingComment.Likes.Remove(existingLike);
        }
        else
        {
            var newLike = new Like
            {
                CommentId = commentId,
                UserId = userId
            };

            existingComment.Likes.Add(newLike);
        }

        existingComment.UpdatedAt = _momentProvider.DateTimeOffsetUtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existingComment.ToModel();
    }

    public async Task<Result<CommentModel>> UpdateAsync(int commentId, (int userId, string userRole) userData,
        UpdateCommentRequestModel request,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Comment, bool>> predicate =
            comment => comment.Id == commentId &&
                       (comment.AuthorId == userData.userId || userData.userRole == "Admin");

        var existingComment = await _commentRepository
            .AsQueryable()
            .Include(comment => comment.Author)
            .Include(comment => comment.Likes)
            .FirstOrDefaultAsync(predicate,
                cancellationToken);

        if (existingComment is null)
        {
            return ErrorCode.CommentNotFound;
        }

        if (string.IsNullOrEmpty(request.Content) || request.Content == existingComment.Content)
        {
            return ErrorCode.NothingToUpdate;
        }

        existingComment.Content = request.Content;
        existingComment.UpdatedAt = _momentProvider.DateTimeOffsetUtcNow;

        _commentRepository.Update(existingComment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existingComment.ToModel();
    }
}