﻿using Blog.Application.Common;
using Blog.Application.Models.Request;
using Blog.Application.Models.Request.Auth;
using Blog.Application.Models.Request.User;
using Blog.Application.Models.Response;
using Blog.Application.Models.Response.User;

namespace Blog.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<None>> CreateAsync(string username, string email, string password,
        CancellationToken cancellationToken = default);
    Task<Result<UserModel>> CreateGoogleAsync(CreateGoogleRequestModel createGoogleRequestModel,
        CancellationToken cancellationToken = default);
    Task<Result<UserModel>> GetCheckedUserAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<Result<UserModel>> UpdateAsync(int userId, UpdateUserRequestModel request, CancellationToken cancellationToken = default);
    Task<Result<None>> DeleteAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<GetUsersResponseModel>> GetAsync(GetUsersRequestModel request, CancellationToken cancellationToken = default);
}