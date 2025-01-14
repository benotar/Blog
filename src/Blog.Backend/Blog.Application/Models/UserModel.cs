using Blog.Application.Models.Abstraction;
using Blog.Domain.Entities;
using Mapster;

namespace Blog.Application.Models;

public record UserModel
{
    public int Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
}