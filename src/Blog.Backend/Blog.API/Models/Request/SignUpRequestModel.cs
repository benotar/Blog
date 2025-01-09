using System.ComponentModel.DataAnnotations;
using Blog.API.Infrastructure;

namespace Blog.API.Models.Request;

public record SignUpRequestModel
{
    [Required]
    [Length(6,20, ErrorMessage = $"{nameof(Username)} must be between 6 and 20 characters")]
    public string Username { get; init; }
    
    [Required]
    [EmailAddress]
    public string Email { get; init; }
    
    [Required]
    [Password]
    public string Password { get; init; }
}