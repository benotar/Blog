using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Blog.Application.Common;
using Blog.Application.Configurations;
using Blog.Application.Extensions;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Blog.Application.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtConfiguration _jwtConfig;
    private readonly IMomentProvider _momentProvider;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;

    public JwtProvider(IOptions<JwtConfiguration> jwtConfig, IMomentProvider momentProvider,
        IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _momentProvider = momentProvider;
        _configuration = configuration;
        _jwtConfig = jwtConfig.Value;
        _unitOfWork = unitOfWork;
        _refreshTokenRepository = _unitOfWork.GetRepository<RefreshToken>();
    }

    public string GenerateToken(UserModel user)
    {
        var secretKey = _configuration.GetSection(_jwtConfig.KeySectionName).Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };

        var expires = _momentProvider.DateTimeUtcNow.AddMinutes(_jwtConfig.AccessExpirationMinutes);

        var securityToken = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            expires: expires,
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
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

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        //_unitOfWorkTemp.RefreshTokenRepository.Add(refreshToken);

        //await _unitOfWorkTemp.SaveChangesAsync(cancellationToken);

        return refreshToken.Token;
    }

    public async Task<Result<RefreshTokenModel>?> GetRefreshTokenEntityAsync(string refreshToken, int userId,
        CancellationToken cancellationToken = default)
    {
        // var existingRefreshToken =
        //     await _unitOfWorkTemp.RefreshTokenRepository.Get(refreshToken, userId,
        //         cancellationToken);

        var existingRefreshToken = await _refreshTokenRepository
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Token == refreshToken, cancellationToken);

        return existingRefreshToken is not null ? existingRefreshToken.ToModel() : ErrorCode.InvalidRefreshToken;
    }

    public async Task<Result<RefreshTokenModel?>> VerifyAndGetRefreshTokenAsync(string refreshToken, int userId,
        CancellationToken cancellationToken = default)
    {
        var existingRefreshToken = await _refreshTokenRepository
            .AsQueryable()
            .Where(refresh => refresh.UserId == userId && refresh.Token == refreshToken)
            .FirstOrDefaultAsync(cancellationToken);
        
        // var existingRefreshToken =
        //     await _unitOfWorkTemp.RefreshTokenRepository.GetIncludeUserAsync(refreshToken, userId,
        //         cancellationToken);

        if (existingRefreshToken is null)
        {
            return ErrorCode.InvalidRefreshToken;
        }

        if (existingRefreshToken.ExpiresOnUtc < _momentProvider.DateTimeOffsetUtcNow)
        {
            return ErrorCode.RefreshTokenHasExpired;
        }

        return existingRefreshToken.ToModel();
    }

    public async Task<Result<string>> UpdateRefreshTokenAsync(int targetTokenId, string targetToken, UserModel user,
        CancellationToken cancellationToken = default)
    {
        var newToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var newExpireOnUtc = _momentProvider.DateTimeOffsetUtcNow.AddDays(_jwtConfig.RefreshExpirationDays);

        var existingRefreshToken = await _refreshTokenRepository
            .FirstOrDefaultAsync(r => r.Id == targetTokenId && r.Token == targetToken, cancellationToken);

        if (existingRefreshToken is null)
        {
            return ErrorCode.InvalidRefreshToken;
        }

        existingRefreshToken.Token = newToken;
        existingRefreshToken.ExpiresOnUtc = newExpireOnUtc;

        _refreshTokenRepository.Update(existingRefreshToken);

        return existingRefreshToken.Token;
        // var updateResult = await _unitOfWorkTemp.RefreshTokenRepository.UpdateAsync(targetTokenId, targetToken,
        //     newToken,
        //     newExpireOnUtc,
        //     cancellationToken);
        //
        // return updateResult.IsSucceed ? newToken : updateResult.ErrorCode;
    }

    public async Task<Result<None>> DeleteRefreshTokensResultAsync(int userId,
        CancellationToken cancellationToken = default)
    {
        var rowsAffected =
            await _refreshTokenRepository.RemoveAsync(refresh => refresh.UserId == userId, cancellationToken);

        return rowsAffected == 0 ? ErrorCode.ThereIsNothingToDelete : new None();
        // var deleteResult = await _unitOfWorkTemp
        //     .RefreshTokenRepository
        //     .DeleteAllAsync(userId, cancellationToken);
        //
        // return deleteResult.IsSucceed ? Result<None>.Success() : deleteResult.ErrorCode;
    }

    public async Task<Result<int>> GetUserIdByRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = await _refreshTokenRepository
            .AsQueryable()
            .Where(refresh => refresh.Token == refreshToken)
            .Select(refresh => refresh.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        // var userId = await _unitOfWorkTemp
        //     .RefreshTokenRepository
        //     .GetUserIdByRefreshTokenAsync(refreshToken, cancellationToken);

        return userId == 0 ? ErrorCode.InvalidRefreshToken : userId;
    }
}