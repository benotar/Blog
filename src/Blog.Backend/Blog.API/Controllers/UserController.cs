using Blog.Application.Common.ValidationAttributes;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models.Request;
using Blog.Application.Models.Request.User;
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
    public async Task<IActionResult> Update([FromRoute] int userId,
        [FromBody] UpdateUserRequestModel request, CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateAsync(userId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("delete/{userId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int userId, CancellationToken cancellationToken = default)
    {
        var result = await _userService.DeleteAsync(userId, cancellationToken);
        return ToActionResult(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-users")]
    public async Task<IActionResult> Get([FromQuery] GetUsersRequestModel request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAsync(request, cancellationToken);
        return ToActionResult(result);
    }
}
