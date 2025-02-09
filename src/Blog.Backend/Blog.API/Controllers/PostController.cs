using Blog.Application.Common;
using Blog.Application.Common.ValidationAttributes;
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

    [ValidateUserId]
    [Authorize(Roles = "Admin")]
    [HttpPut("update-post/{postId:int}/{userId:int}")]
    public async Task<Result<PostModel>> Update([FromRoute] int postId, [FromRoute] int userId,
        UpdatePostRequestModel request, CancellationToken cancellationToken = default)
    {
        return await _postService.UpdatePostAsync(userId, postId, request,  cancellationToken);
    }

    [ValidateUserId]
    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-post/{postId:int}/{userId:int}")]
    public async Task<Result<None>> Delete([FromRoute] int postId, [FromRoute] int userId,
        CancellationToken cancellationToken = default)
    {
        return await _postService.DeletePostAsync(userId, postId, cancellationToken);
    }

    [HttpGet("get-categories")]
    public Result<IEnumerable<PostCategory>> GetCategories()
    {
        return _postService.GetCategories();
    }
}