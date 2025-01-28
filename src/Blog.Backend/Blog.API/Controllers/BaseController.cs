using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    internal int UserId => int.Parse(User.Claims
        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
}