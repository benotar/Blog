using Blog.Application.Common;
using Blog.Application.Common.ValidationAttributes;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Blog.Application.Models.Request.User;
using Blog.Application.Models.Response;
using Blog.Application.Models.Response.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [ValidateUserId]
    [HttpPut("update/{userId:int}")]
    public async Task<Result<UserModel>> Update([FromRoute] int userId,
        [FromBody] UpdateUserRequestModel request, CancellationToken cancellationToken = default)
    {
        var updateUserResult = await _userService
            .UpdateAsync(userId, request, cancellationToken);

        if (!updateUserResult.IsSucceed)
        {
            return updateUserResult.ErrorCode;
        }

        return updateUserResult.Payload;
    }

    [ValidateUserId]
    [HttpDelete("delete/{userId:int}")]
    public async Task<Result<None>> Delete([FromRoute] int userId, CancellationToken cancellationToken = default)
    {
        return await _userService.DeleteAsync(userId, cancellationToken);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-users")]
    public async Task<Result<GetUsersResponseModel>> Get([FromQuery] GetUsersRequestModel request, 
        CancellationToken cancellationToken = default)
    {
        return await _userService.GetAsync(request, cancellationToken);
    }
}