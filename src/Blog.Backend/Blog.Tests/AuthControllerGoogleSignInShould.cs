using Blog.API.Controllers;
using Blog.API.Models.Request;
using Blog.API.Models.Response;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Services;
using Blog.Application.Models;
using Blog.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Blog.Tests;

public class AuthControllerGoogleSignInShould
{
    private readonly AuthController _sut;
    private readonly Mock<IGoogleService> _googleServiceMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;
    private readonly Mock<ICookieProvider> _cookieProviderMock;
    private readonly GoogleSignInRequestModel _request;
    private readonly GoogleSignInResponseModel _response;
    private readonly UserModel _expectedUserFromGoogleService;
    private readonly CancellationToken _clt;
    
    public AuthControllerGoogleSignInShould()
    {
        var userServiceMock = new Mock<IUserService>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _cookieProviderMock = new Mock<ICookieProvider>();
        _googleServiceMock = new Mock<IGoogleService>();

        _sut = new AuthController(userServiceMock.Object, _cookieProviderMock.Object,
            _jwtProviderMock.Object, _googleServiceMock.Object);

        _request = new GoogleSignInRequestModel
        {
            Email = "email",
            Name = "username",
            ProfilePictureUrl = "picture",
        };
        
        _expectedUserFromGoogleService = new UserModel
        {
            Id = 1,
            Email = _request.Email,
            Username = "translated_username228",
            ProfilePictureUrl = _request.ProfilePictureUrl
        };
        
        _response = new GoogleSignInResponseModel
        {
            Id = _expectedUserFromGoogleService.Id,
            Email = _expectedUserFromGoogleService.Email,
            Username = _expectedUserFromGoogleService.Username,
            ProfilePictureUrl = _request.ProfilePictureUrl
        };
        
        _clt = CancellationToken.None;
    }

    [Fact]
    public async Task ReturnError_WhenTokenTypeInvalid()
    {
        // Arrange
        _googleServiceMock.Setup(g =>
                g.GetOrCreateUserFromGoogleCredentialsAsync(_request.Email, _request.Name, _request.ProfilePictureUrl,
                    _clt))
            .ReturnsAsync(_expectedUserFromGoogleService);
        
        _jwtProviderMock.Setup(j =>
                j.GenerateToken(_expectedUserFromGoogleService.Id, _expectedUserFromGoogleService.Email, JwtType.Access))
            .Returns(ErrorCode.JwtTokenIsUndefined);
        
        _jwtProviderMock.Setup(j =>
                j.GenerateToken(_expectedUserFromGoogleService.Id, _expectedUserFromGoogleService.Email, JwtType.Refresh))
            .Returns(ErrorCode.JwtTokenIsUndefined);
        
        // Act
        var actual = await _sut.GoogleSignIn(_request, _clt);
        
        // Assert
        actual.IsSucceed.Should().BeFalse();
        actual.ErrorCode.Should().Be(ErrorCode.JwtTokenIsUndefined);
        actual.Payload.Should().BeNull();
    }

    [Fact]
    public async Task ReturnUser_WhenGoogleSignInIsSuccessful()
    {
        // Arrange
        var accessToken = "access_token";
        var refreshToken = "refreshToken";
        var httpContext = new DefaultHttpContext();
        _sut.ControllerContext.HttpContext = httpContext;

        _googleServiceMock.Setup(g =>
                g.GetOrCreateUserFromGoogleCredentialsAsync(_request.Email, _request.Name, _request.ProfilePictureUrl,
                    _clt))
            .ReturnsAsync(_expectedUserFromGoogleService);

        _jwtProviderMock.Setup(j =>
                j.GenerateToken(_expectedUserFromGoogleService.Id, _expectedUserFromGoogleService.Email, JwtType.Access))
            .Returns(accessToken);
        _jwtProviderMock.Setup(j =>
                j.GenerateToken(_expectedUserFromGoogleService.Id, _expectedUserFromGoogleService.Email, JwtType.Refresh))
            .Returns(refreshToken);
        _cookieProviderMock.Setup(c => c.AddTokens(httpContext.Response, accessToken,
            refreshToken));

        // Act
        var actual = await _sut.GoogleSignIn(_request, _clt);

        // Assert
        actual.IsSucceed.Should().BeTrue();
        actual.ErrorCode.Should().BeNull();
        actual.Payload.Should().BeOfType<GoogleSignInResponseModel>()
            .Which.Should().Be(_response);

        _googleServiceMock.Verify(s => s.GetOrCreateUserFromGoogleCredentialsAsync(_request.Email, _request.Name,
            _request.ProfilePictureUrl, _clt), Times.Once);

        _jwtProviderMock.Verify(j => j.GenerateToken(_expectedUserFromGoogleService.Id,
            _expectedUserFromGoogleService.Email,
            JwtType.Access), Times.Once);
        _jwtProviderMock.Verify(j => j.GenerateToken(_expectedUserFromGoogleService.Id,
            _expectedUserFromGoogleService.Email,
            JwtType.Refresh), Times.Once);
        _cookieProviderMock.Verify(c => c.AddTokens(httpContext.Response, accessToken,
            refreshToken), Times.Once);
    }
}