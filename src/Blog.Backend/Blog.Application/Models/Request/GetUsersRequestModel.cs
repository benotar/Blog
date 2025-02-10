namespace Blog.Application.Models.Request;

public sealed record GetUsersRequestModel(
    string? SortOrder,
    int? StartIndex,
    int? Limit);