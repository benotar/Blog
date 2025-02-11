using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Models.Request;

public record CreateCommentRequestModel
{
    [Required] [MinLength(5)]
    public string Content { get; init; }
    
    [Required][Range(1, int.MaxValue)]
    public int PostId { get; init; }
}