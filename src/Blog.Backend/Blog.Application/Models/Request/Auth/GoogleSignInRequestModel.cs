using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Models.Request.Auth;

public record GoogleSignInRequestModel
{
    [Required] [EmailAddress] public string Email { get; init; }

    [Required] public string Name { get; init; }

    [Required] public string ProfilePictureUrl { get; init; }
}