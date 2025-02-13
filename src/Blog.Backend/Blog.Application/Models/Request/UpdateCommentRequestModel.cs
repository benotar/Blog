using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Models.Request;

public record UpdateCommentRequestModel
{
    [Required] [MinLength(5)] public string Content { get; init; }
};