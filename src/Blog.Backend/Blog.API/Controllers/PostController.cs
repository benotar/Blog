using Blog.Application.Common.ValidationAttributes;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[Authorize(Roles = "Admin")]
public class PostController : BaseController
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _postService.CreateAsync(UserId, request, cancellationToken);
        return ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpGet("get-posts")]
    public async Task<IActionResult> Get([FromQuery] GetPostsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _postService.GetPostsAsync(request, cancellationToken);
        return ToActionResult(result);
    }

    [ValidateUserId]
    [HttpPut("update-post/{postId:int}/{userId:int}")]
    public async Task<IActionResult> Update([FromRoute] int postId, [FromRoute] int userId,
        UpdatePostRequestModel request, CancellationToken cancellationToken = default)
    {
        var result = await _postService.UpdatePostAsync(userId, postId, request, cancellationToken);
        return ToActionResult(result);
    }

    [ValidateUserId]
    [HttpDelete("delete-post/{postId:int}/{userId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int postId, [FromRoute] int userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _postService.DeletePostAsync(userId, postId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("get-categories")]
    public IActionResult GetCategories()
    {
        var result = _postService.GetCategories();
        return ToActionResult(result);
    }
}
