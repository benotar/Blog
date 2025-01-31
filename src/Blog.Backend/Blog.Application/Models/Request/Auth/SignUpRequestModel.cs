using System.ComponentModel.DataAnnotations;
using Blog.Application.Common.ValidationAttributes;

namespace Blog.Application.Models.Request.Auth;

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