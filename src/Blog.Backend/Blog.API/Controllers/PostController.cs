﻿using Blog.Application.Common;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
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
    public async Task<Result<PostModel>> Create([FromBody] CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        return await _postService.CreateAsync(UserId, request, cancellationToken);
    }
}