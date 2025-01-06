using System.ComponentModel.DataAnnotations;

namespace Blog.API.Models.Request;

public class SignUpRequestModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
}