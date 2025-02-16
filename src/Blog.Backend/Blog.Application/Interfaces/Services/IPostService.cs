﻿using Blog.Application.Common;
using Blog.Application.Models.Request;
using Blog.Application.Models.Response;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Services;

public interface IPostService
{
    Task<Result<PostModel>> CreateAsync(int userId, CreatePostRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<GetPostsResponseModel>> GetPostsAsync(GetPostsRequestModel request,
        CancellationToken cancellationToken = default);

    Task<Result<None>> DeletePostAsync(int userId, int postId, CancellationToken cancellationToken = default);

    Task<Result<PostModel>> UpdatePostAsync(int userId, int postId, UpdatePostRequestModel request,
        CancellationToken cancellationToken = default);

    Result<IEnumerable<PostCategory>> GetCategories();
}