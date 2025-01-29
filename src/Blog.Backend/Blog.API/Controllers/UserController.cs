using Blog.API.Extensions;
using Blog.API.Infrastructure;
using Blog.API.Models.Request.User;
using Blog.API.Models.Response.User;
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

    [HttpPut("update/{userId:int}")]
    public async Task<Result<UserResponseModel>> Update(int userId, [FromBody] UpdateUserRequestModel request)
    {
        var updateUserResult = await _userService
            .UpdateAsync(userId, request.Username, request.Email, request.ProfilePictureUrl,
                request.CurrentPassword, request.NewPassword);

        if (!updateUserResult.IsSucceed)
        {
            return updateUserResult.ErrorCode;
        }

        return updateUserResult.Payload.ToModel();
    }
}