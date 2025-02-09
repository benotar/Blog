namespace Blog.Application.Models.Request;

public record GetPostsRequestModel(
    int? UserId,
    int? PostId,
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int? StartIndex,
    int? Limit);