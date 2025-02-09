using System.ComponentModel.DataAnnotations;
using Blog.Domain.Enums;

namespace Blog.Application.Models.Request;

public sealed record CreatePostRequest
{
    [Required]
    public string Content { get; init; }
    
    [Required]
    [MinLength(3)]
    public string Title { get; init; }
    
    [Url]
    public string? ImageUrl { get; init; }
    
    [Required] 
    public PostCategory Category { get; init; }
}