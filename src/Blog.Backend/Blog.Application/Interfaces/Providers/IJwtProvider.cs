using Blog.Application.Common;
using Blog.Domain.Enums;

namespace Blog.Application.Interfaces.Providers;

public interface IJwtProvider
{
    Result<string> GenerateToken(int userId, string email, JwtType jwtType);
}