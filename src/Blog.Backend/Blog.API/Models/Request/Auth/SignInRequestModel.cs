using System.ComponentModel.DataAnnotations;

namespace Blog.API.Models.Request.Auth;

public record SignInRequestModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    public string Password { get; init; }
}