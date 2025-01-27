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
    private readonly GoogleSignInRequestModel _request;
    private readonly GoogleSignInResponseModel _response;
    private readonly UserModel _expectedUserFromGoogleService;
    private readonly CancellationToken _clt;
    private readonly string _accessToken;
    private readonly string _refreshToken;

    public AuthControllerGoogleSignInShould()
    {
        var userServiceMock = new Mock<IUserService>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        _googleServiceMock = new Mock<IGoogleService>();

        _sut = new AuthController(userServiceMock.Object, _jwtProviderMock.Object, _googleServiceMock.Object);

        _accessToken = "access_token";
        _refreshToken = "refreshToken";
        var tokensResponse = new TokensResponseModel(_accessToken, _refreshToken);
        
        _request = new GoogleSignInRequestModel
        {
            Email = "email",
            Name = "username",
            ProfilePictureUrl = "picture"
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
            ProfilePictureUrl = _request.ProfilePictureUrl,
            Tokens = tokensResponse
        };

        _clt = CancellationToken.None;
    }

    [Fact]
    public async Task ReturnUser_WhenGoogleSignInIsSuccessful()
    {
        // Arrange

        _googleServiceMock.Setup(g =>
                g.FindOrCreateGoogleUserAsync(_request.Email, _request.Name, _request.ProfilePictureUrl,
                    _clt))
            .ReturnsAsync(_expectedUserFromGoogleService);

        _jwtProviderMock.Setup(j =>
                j.GenerateToken(_expectedUserFromGoogleService))
            .Returns(_accessToken);

        _jwtProviderMock.Setup(j =>
                j.CreateRefreshTokenAsync(_expectedUserFromGoogleService, _clt))
            .ReturnsAsync(_refreshToken);

        // Act
        var actual = await _sut.GoogleSignIn(_request, _clt);

        // Assert
        actual.IsSucceed.Should().BeTrue();
        actual.ErrorCode.Should().BeNull();
        actual.Payload.Should().BeOfType<GoogleSignInResponseModel>()
            .Which.Should().Be(_response);

        _googleServiceMock.Verify(s => s.FindOrCreateGoogleUserAsync(_request.Email, _request.Name,
            _request.ProfilePictureUrl, _clt), Times.Once);
    }
}