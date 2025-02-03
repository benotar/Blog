using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    internal int UserId => int.Parse(User.Claims
        .First(claim => claim.Type == ClaimTypes.Name)?.Value);
}