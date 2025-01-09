using Blog.Application.Configurations;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Providers;
using Blog.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace Blog.Tests;

public class JwtProviderGenerateTokenShould
{
    private readonly JwtProvider _sut;

    public JwtProviderGenerateTokenShould()
    {
        var jwtConfigMock = new Mock<IOptions<JwtConfiguration>>();

        var momentProviderMock = new Mock<IMomentProvider>();

        _sut = new JwtProvider(jwtConfigMock.Object, momentProviderMock.Object);
    }

    [Fact]
    public void ReturnErrorCode_WhenUndefinedTokenTypeIsProvided()
    {
        // Assert
        var undefinedTokenType = JwtType.Undefined;

        // Act
        var result = _sut.GenerateToken(It.IsAny<int>(),It.IsAny<string>() ,undefinedTokenType);

        // Assert
        result.IsSucceed.Should().BeFalse();
        result.Data.Should().BeNull();
        result.ErrorCode.Should().Be(ErrorCode.JwtTokenIsUndefined);
    }
}