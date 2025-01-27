using System.ComponentModel.DataAnnotations;

namespace Blog.API.Models.Request;

public record RefreshTokenRequestModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int UserId {get; init;}
    [Required] public string RefreshToken {get; init;}
}