using System.ComponentModel.DataAnnotations;

namespace Blog.API.Models.Request;

public record RefreshTokenRequestModel
{
    [Required] public int UserId {get; init;}
    [Required] public string RefreshToken {get; init;}
}