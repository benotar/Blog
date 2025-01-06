using Blog.API.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

public class AuthController : BaseController
{
    [HttpPost]
    public IActionResult SignUp([FromBody] SignUpRequestModel model)
    {
        return Ok(model);
    }
}