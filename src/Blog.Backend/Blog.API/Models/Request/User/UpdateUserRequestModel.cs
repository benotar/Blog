using System.ComponentModel.DataAnnotations;

namespace Blog.API.Models.Request.User;

public record UpdateUserRequestModel
{
    [Required] 
    [Length(6,20, ErrorMessage = $"{nameof(Username)} must be between 6 and 20 characters")]
    public string Username {get; init; }
    
    [Required]
    [EmailAddress]
    public string Email {get; init; }
    
    [Required]
   // [Url]
    public string ProfilePictureUrl {get; init; }
}