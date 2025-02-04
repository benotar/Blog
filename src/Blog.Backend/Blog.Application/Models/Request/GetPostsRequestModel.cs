namespace Blog.Application.Models.Request;

public record GetPostsRequestModel(
    int? UserId,
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int Page,
    int PageSize);