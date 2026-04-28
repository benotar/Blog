using System.Security.Claims;
using Blog.Application.Common;
using Blog.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    internal int UserId => int.Parse(User.Claims
        .First(claim => claim.Type == ClaimTypes.Name)?.Value);

    internal string UserRole => User.Claims
        .First(claim => claim.Type == ClaimTypes.Role)?.Value;

    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        return result.ErrorCode switch
        {
            null => Ok(result),
            ErrorCode.UserNotFound or ErrorCode.PostNotFound or ErrorCode.CommentNotFound => NotFound(result),
            ErrorCode.UserAlreadyExist or ErrorCode.PostTitleAlreadyExist or ErrorCode.UsernameAlreadyExist
                or ErrorCode.EmailAlreadyExist => Conflict(result),
            ErrorCode.InvalidCredentials or ErrorCode.UserUnauthenticated or ErrorCode.InvalidRefreshToken
                or ErrorCode.RefreshTokenHasExpired => Unauthorized(result),
            ErrorCode.AccessDenied => Forbid(),
            _ => BadRequest(result)
        };
    }
}
