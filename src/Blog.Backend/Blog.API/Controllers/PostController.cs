using Blog.Application.Common;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[Authorize]
public class PostController : BaseController
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create")]
    public async Task<Result<PostModel>> Create([FromBody] CreatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _postService.CreateAsync(UserId, request, cancellationToken);
    }

    [AllowAnonymous]
    [HttpGet("get-posts")]
    public async Task<Result<GetPostsResponseModel>> Get([FromQuery] GetPostsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _postService.GetPostsAsync(request, cancellationToken);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-post/{postId:int}")]
    public async Task<Result<None>> Get([FromRoute] int postId,
        CancellationToken cancellationToken = default)
    {
        return await _postService.DeletePostAsync(UserId, postId, cancellationToken);
    }

    [HttpGet("get-categories")]
    public Result<IEnumerable<PostCategory>> GetCategories()
    {
        return _postService.GetCategories();
    }
}