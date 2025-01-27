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

public class AuthControllerSignInShould
{
    private readonly AuthController _sut;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IJwtProvider> _jwtProviderMock;

    public AuthControllerSignInShould()
    {
        _userServiceMock = new Mock<IUserService>();
        _jwtProviderMock = new Mock<IJwtProvider>();
        var googleServiceMock = new Mock<IGoogleService>();
        
        _sut = new AuthController(_userServiceMock.Object, _jwtProviderMock.Object, googleServiceMock.Object);
    }

    [Fact]
    public async Task ReturnInvalidCredentialsError_WhenInvalidCredentials()
    {
        var request = new SignInRequestModel
        {
            Email = "email",
            Password = "password",
        };

        _userServiceMock.Setup(s => s.GetCheckedUserAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ErrorCode.InvalidCredentials);

        var actual = await _sut.Login(request, It.IsAny<CancellationToken>());

        actual.IsSucceed.Should().BeFalse();
        actual.ErrorCode.Should().Be(ErrorCode.InvalidCredentials);
        actual.Payload.Should().BeNull();
    }

    [Fact]
    public async Task ReturnCurrentUser_WhenSignedIn()
    {
        // Arrange
        var request = new SignInRequestModel
        {
            Email = "email",
            Password = "password",
        };

        var accessToken = "access_token";
        var refreshToken = "refreshToken";
        var tokensResponse = new TokensResponseModel(accessToken, refreshToken);

        var clt = CancellationToken.None;
        
        var expectedUserFromService = new UserModel
        {
            Email = request.Email,
            Username = "username",
            ProfilePictureUrl = "url"
        };

        var expectedSignInResponse = new SignInResponseModel
        {
            Email = expectedUserFromService.Email,
            Username = expectedUserFromService.Username,
            ProfilePictureUrl = expectedUserFromService.ProfilePictureUrl,
            Tokens = tokensResponse
        };

        _userServiceMock.Setup(s => s.GetCheckedUserAsync(
                request.Email, request.Password, clt))
            .ReturnsAsync(expectedUserFromService);
        
        _jwtProviderMock.Setup(j =>
                j.GenerateToken(expectedUserFromService))
            .Returns(accessToken);

        _jwtProviderMock.Setup(j =>
                j.CreateRefreshTokenAsync(expectedUserFromService, clt))
            .ReturnsAsync(refreshToken);
        
        // Act
        var actual = await _sut.Login(request, clt);

        // Assert
        actual.IsSucceed.Should().BeTrue();
        actual.ErrorCode.Should().BeNull();
        actual.Payload.Should().BeOfType<SignInResponseModel>()
            .Which.Should().Be(expectedSignInResponse);

        _userServiceMock.Verify(s => s.GetCheckedUserAsync(
            request.Email, request.Password, It.IsAny<CancellationToken>()), Times.Once);
    }
}