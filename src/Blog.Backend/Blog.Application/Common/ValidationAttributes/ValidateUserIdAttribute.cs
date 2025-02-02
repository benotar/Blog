using System.Security.Claims;
using Blog.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Application.Common.ValidationAttributes;

public class ValidateUserIdAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.RouteData.Values.TryGetValue("userId", out var userIdFromRoute))
        {
            context.Result = new BadRequestObjectResult(Result<None>.Error(ErrorCode.UserIdMissing));
            return;
        }

        var userIdFromClaim = context.HttpContext.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (userIdFromClaim == null)
        {
            context.Result = new UnauthorizedObjectResult(Result<None>.Error(ErrorCode.UserIdMissing));
            return;
        }

        if (userIdFromClaim != userIdFromRoute.ToString())
        {
            context.Result = new ObjectResult(Result<None>.Error(ErrorCode.InvalidUserId))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }
    }
}