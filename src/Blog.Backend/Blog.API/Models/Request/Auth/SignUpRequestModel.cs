using System.ComponentModel.DataAnnotations;
using Blog.API.Infrastructure;

namespace Blog.API.Models.Request.Auth;

public record SignUpRequestModel
{
    [Required]
    [Username]
    public string Username { get; init; }
    
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    [Password]
    public string Password { get; init; }
}