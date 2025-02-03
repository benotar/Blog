using System.Text.RegularExpressions;
using Blog.Application.Common;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services;

public partial class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Post> _postRepository;

    public PostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
            Slug = slug
        };

        if (!string.IsNullOrEmpty(request.ImageUrl))
        {
            newPost.ImageUrl = request.ImageUrl;
        }

        await _postRepository.AddAsync(newPost, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newPost.ToModel();
    }

    [GeneratedRegex("[^a-zA-Z0-9-]")]
    private static partial Regex MyRegex();
}