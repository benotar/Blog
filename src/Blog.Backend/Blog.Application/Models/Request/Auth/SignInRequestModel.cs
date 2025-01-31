using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Models.Request.Auth;

public record SignInRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    public string Password { get; init; }
}