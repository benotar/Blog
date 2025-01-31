using System.ComponentModel.DataAnnotations;
using Blog.Application.Common.ValidationAttributes;

namespace Blog.Application.Models.Request.User;

public record UpdateUserRequestModel
{
    [Username] 
    public string? Username { get; init; }
    [EmailAddress] 
    public string? Email { get; init; }
    [Url] 
    public string? ProfilePictureUrl { get; init; }

    public string? CurrentPassword { get; init; }

    [Password] 
    public string? NewPassword { get; init; }
}