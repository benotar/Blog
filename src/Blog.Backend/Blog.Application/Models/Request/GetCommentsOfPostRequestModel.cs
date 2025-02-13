using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Models.Request;

public sealed record GetCommentsOfPostRequestModel
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}