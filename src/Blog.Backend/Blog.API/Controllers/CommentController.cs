﻿using Blog.Application.Common;
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
    public async Task<Result<PagedList<CommentModel>>> Get([FromRoute] int postId,
        [FromQuery] GetCommentsOfPostRequestModel request, CancellationToken cancellationToken = default)
    {
        return await _commentService.GetAsync(postId, request, cancellationToken);
    }

    [HttpPut("like-comment/{commentId:int}")]
    public async Task<Result<CommentModel>> LikeComment([FromRoute] int commentId,
        CancellationToken cancellationToken = default)
    {
        return await _commentService.LikeAsync(UserId, commentId, cancellationToken);
    }

    [HttpPut("update-comment/{commentId:int}")]
    public async Task<Result<CommentModel>> UpdateComment([FromRoute] int commentId,
        [FromBody] UpdateCommentRequestModel request, CancellationToken cancellationToken = default)
    {
        return await _commentService.UpdateAsync(commentId, (UserId, UserRole), request, cancellationToken);
    }

    [HttpDelete("delete-comment/{commentId:int}")]
    public async Task<Result<None>> DeleteComment([FromRoute] int commentId,
        CancellationToken cancellationToken = default)
    {
        return await _commentService.DeleteAsync(commentId, (UserId, UserRole), cancellationToken);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-comments")]
    public async Task<Result<PagedList<CommentModel>>> GetAll([FromQuery] GetCommentsRequestModel request,
        CancellationToken cancellationToken = default)
    {
        return await _commentService.GetAsync(request, cancellationToken);
    }
}