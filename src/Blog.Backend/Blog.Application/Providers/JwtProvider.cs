using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Application.Common;
using Blog.Application.Configurations;
using Blog.Application.Interfaces.Providers;
using Blog.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Application.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtConfiguration _jwtConfig;
    private readonly IMomentProvider _momentProvider;

    public JwtProvider(IOptions<JwtConfiguration> jwtConfig, IMomentProvider momentProvider)
    {
        _momentProvider = momentProvider;
        _jwtConfig = jwtConfig.Value;
    }

    public Result<string> GenerateToken(int userId, string email, JwtType jwtType)
    {
        if (jwtType == JwtType.Undefined)
        {
            return ErrorCode.JwtTokenIsUndefined;
        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Typ, jwtType.ToString())
        };
        
        var expires = jwtType switch
        {
            JwtType.Access => _momentProvider.DateTimeUtcNow.AddMinutes(_jwtConfig.AccessExpirationMinutes),
            JwtType.Refresh => _momentProvider.DateTimeUtcNow.AddDays(_jwtConfig.AccessExpirationMinutes)
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            expires: expires,
            claims: claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}