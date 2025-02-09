using System.ComponentModel.DataAnnotations;
using Blog.Domain.Enums;

namespace Blog.Application.Models.Request;

public sealed record UpdatePostRequestModel
{
    [MinLength(3)] public string? Title { get; init; }
    public PostCategory? Category { get; init; }
    public string? Content { get; init; }
    [Url] public string? ImageUrl { get; init; }
}