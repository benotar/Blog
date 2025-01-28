using Blog.API.Infrastructure;
using Blog.API.Models.Request.User;
using Blog.Application.Common;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[Authorize]
[ValidateUserId]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("update/{userId:int}")]
    public async Task<Result<None>> Update(int userId, [FromBody] UpdateUserRequestModel request)
    {
        return await _userService
            .UpdateAsync(userId, request.Username, request.Email, request.ProfilePictureUrl);
    }
}