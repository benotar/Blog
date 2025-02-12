using Blog.Application.Common;
using Blog.Application.Common.ValidationAttributes;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
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
    public async Task<Result<CommentModel>> Create([FromRoute] int userId, [FromBody] CreateCommentRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _commentService.CreateAsync(userId, request, cancellationToken);
    }

    [AllowAnonymous]
    [HttpGet("get-comments/{postId:int}")]
    public async Task<Result<IEnumerable<CommentDetailModel>>> Get([FromRoute]int postId, CancellationToken cancellationToken = default)
    {
        return await _commentService.GetAsync(postId, cancellationToken);
    }
}