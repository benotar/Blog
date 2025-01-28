using System.ComponentModel.DataAnnotations;
using Blog.API.Infrastructure;

namespace Blog.API.Models.Request.User;

public record UpdateUserRequestModel
{
    [Username] 
    public string? Username { get; init; }
    [EmailAddress] public string? Email { get; init; }
    [Url] public string? ProfilePictureUrl { get; init; }
}