using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Blog.Application.Common;
using Blog.Application.Configurations;
using Blog.Application.Extensions;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Blog.Application.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtConfiguration _jwtConfig;
    private readonly IMomentProvider _momentProvider;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public JwtProvider(IOptions<JwtConfiguration> jwtConfig, IMomentProvider momentProvider,
        IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _momentProvider = momentProvider;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _jwtConfig = jwtConfig.Value;
    }

    public Result<string> GenerateToken(int userId, string email, JwtType jwtType)
    {
        if (jwtType == JwtType.Undefined)
        {
            return ErrorCode.RefreshTokenHasExpired;
        }

        var jwtKey = _configuration.GetSection(_jwtConfig.KeySectionName).Value;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
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
            JwtType.Refresh => _momentProvider.DateTimeUtcNow.AddDays(_jwtConfig.RefreshExpirationDays)
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

    public string GenerateToken(UserModel user)
    {
        var secretKey = _configuration.GetSection(_jwtConfig.KeySectionName).Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            ]),
            Expires = _momentProvider.DateTimeUtcNow.AddMinutes(_jwtConfig.AccessExpirationMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
        };

        var handler = new JsonWebTokenHandler();

        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public async Task<string> CreateRefreshTokenAsync(UserModel user,
        CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresOnUtc = _momentProvider.DateTimeOffsetUtcNow.AddDays(_jwtConfig.RefreshExpirationDays)
        };

        _unitOfWork.RefreshTokenRepository.Add(refreshToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return refreshToken.Token;
    }
    
    public async Task<Result<RefreshTokenModel?>> VerifyAndGetRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var existingRefreshToken =
            await _unitOfWork.RefreshTokenRepository.GetRefreshTokenIncludeUserAsync(refreshToken, cancellationToken);

        if (existingRefreshToken is null || existingRefreshToken.ExpiresOnUtc < _momentProvider.DateTimeOffsetUtcNow)
        {
            return ErrorCode.RefreshTokenHasExpired;
        }

        return existingRefreshToken.ToModel();
    }

    public async Task<Result<string>> UpdateRefreshTokenAsync(string targetToken, UserModel user, CancellationToken cancellationToken = default)
    {
        var newToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var newExpireOnUtc = _momentProvider.DateTimeOffsetUtcNow.AddDays(_jwtConfig.RefreshExpirationDays);
        
        var updateResult =  await _unitOfWork.RefreshTokenRepository.UpdateAsync(targetToken, newToken, newExpireOnUtc,
            cancellationToken);

        return updateResult.IsSucceed ? newToken : updateResult.ErrorCode;
    }
}