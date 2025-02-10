using Blog.Application.Models.Response.User;

namespace Blog.Application.Models.Response;

public sealed record GetUsersResponseModel
{
    public PagedList<UserModel> Data { get; init; }
    public int LastMonthUsersCount { get; init; }
}