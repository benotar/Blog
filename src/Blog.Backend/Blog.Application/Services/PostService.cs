using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.Application.Services;

public partial class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Post> _postRepository;
    private readonly IMomentProvider _momentProvider;

    public PostService(IUnitOfWork unitOfWork, IMomentProvider momentProvider)
    {
        _unitOfWork = unitOfWork;
        _momentProvider = momentProvider;
        _postRepository = _unitOfWork.GetRepository<Post>();
    }

    public async Task<Result<PostModel>> CreateAsync(int userId, CreatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _postRepository.AnyAsync(post => post.Title == request.Title, cancellationToken))
        {
            return ErrorCode.PostTitleAlreadyExists;
        }

        var userRepo = _unitOfWork.GetRepository<User>();

        if (!await userRepo.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return ErrorCode.UserNotFound;
        }

        var slug = MyRegex().Replace(request.Title
            .Replace(" ", "-")
            .ToLower(), string.Empty);

        var newPost = new Post
        {
            UserId = userId,
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            Slug = slug,
            CreatedAt = _momentProvider.DateTimeOffsetUtcNow
        };

        if (!string.IsNullOrEmpty(request.ImageUrl))
        {
            newPost.ImageUrl = request.ImageUrl;
        }

        await _postRepository.AddAsync(newPost, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newPost.ToModel();
    }

    public async Task<Result<GetPostsResponseModel>> GetPostsAsync(GetPostsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var postsQuery = _postRepository.AsQueryable();

        var lastMonthPostsCount = await postsQuery.Where(post =>
                post.CreatedAt >= _momentProvider.DateTimeOffsetUtcNow.AddMonths(-1))
            .CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            if (int.TryParse(request.UserId, out var searchUserId))
            {
                postsQuery = postsQuery.Where(post => post.UserId == searchUserId);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            postsQuery = postsQuery.Where(post =>
                EF.Functions.ILike(post.Category.ToString(), $"%{request.SearchTerm}%") ||
                EF.Functions.ILike(post.Slug, $"%{request.SearchTerm}%") ||
                EF.Functions.ILike(post.Title, $"%{request.SearchTerm}%") ||
                EF.Functions.ILike(post.Content, $"%{request.SearchTerm}%"));
        }

        postsQuery = request.SortOrder?.ToLower() == "desc"
            ? postsQuery.OrderByDescending(GetSortProperty(request))
            : postsQuery.OrderBy(GetSortProperty(request));
        
        var postModels = postsQuery.Select(post => post.ToModel());

        var responseItems = await PagedList<PostModel>.CreateAsync(
            postModels,
            request.Page,
            request.PageSize,
            cancellationToken);
        
        
        
        return new GetPostsResponseModel
        {
            Data =  responseItems,
            LastMonthPostsCount =  lastMonthPostsCount
        };
    }

    public Result<IEnumerable<PostCategory>> GetCategories()
    {
        return Enum.GetValues<PostCategory>();
    }

    [GeneratedRegex("[^a-zA-Z0-9-]")]
    private static partial Regex MyRegex();

    private static Expression<Func<Post, object>> GetSortProperty(GetPostsRequestModel request)
    {
        return request.SortColumn?.ToLower() switch
        {
            "userid" => post => post.UserId,
            "content" => post => post.Content,
            "title" => post => post.Title,
            "category" => post => post.Category,
            "slug" => post => post.Slug,
            _ => post => post.UpdatedAt ?? DateTimeOffset.MinValue
        };
    }
}