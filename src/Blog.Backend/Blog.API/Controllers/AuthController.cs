using Blog.API.Models.Request;
using Blog.Application.Common;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

public class AuthController : BaseController
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("sign-up")]
    public async Task<Result<None>> SignUp([FromBody] SignUpRequestModel model)
    {
        return await _userService.CreateAsync(model.Username, model.Email, model.Password);
    }
}