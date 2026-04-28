using Blog.Application.Common.ValidationAttributes;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[Authorize]
public class CommentController : BaseController
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [ValidateUserId]
    [HttpPost("create-comment/{userId:int}")]
    public async Task<IActionResult> Create([FromRoute] int userId, [FromBody] CreateCommentRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _commentService.CreateAsync(userId, request, cancellationToken);
        return ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpGet("get-comments/{postId:int}")]
    public async Task<IActionResult> Get([FromRoute] int postId,
        [FromQuery] GetCommentsOfPostRequestModel request, CancellationToken cancellationToken = default)
    {
        var result = await _commentService.GetAsync(postId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPut("like-comment/{commentId:int}")]
    public async Task<IActionResult> LikeComment([FromRoute] int commentId,
        CancellationToken cancellationToken = default)
    {
        var result = await _commentService.LikeAsync(UserId, commentId, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPut("update-comment/{commentId:int}")]
    public async Task<IActionResult> UpdateComment([FromRoute] int commentId,
        [FromBody] UpdateCommentRequestModel request, CancellationToken cancellationToken = default)
    {
        var result = await _commentService.UpdateAsync(commentId, (UserId, UserRole), request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("delete-comment/{commentId:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int commentId,
        CancellationToken cancellationToken = default)
    {
        var result = await _commentService.DeleteAsync(commentId, (UserId, UserRole), cancellationToken);
        return ToActionResult(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-comments")]
    public async Task<IActionResult> GetAll([FromQuery] GetCommentsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _commentService.GetAsync(request, cancellationToken);
        return ToActionResult(result);
    }
}
