using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Application.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Blog.Tests;

public class UserServiceGetCheckedUserAsyncShould
{
    private readonly UserService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEncryptionProvider> _encryptionProviderMock;

    public UserServiceGetCheckedUserAsyncShould()
    {
        var mockMomentProvider = new Mock<IMomentProvider>();

        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _encryptionProviderMock = new Mock<IEncryptionProvider>();
        _sut = new UserService(mockMomentProvider.Object, _unitOfWorkMock.Object,
            _encryptionProviderMock.Object);
    }

    [Fact]
    public async Task ReturnInvalidCredentialsErrorCode_WhenUserNotFound()
    {
        _unitOfWorkMock
            .Setup(u => u.UserRepository.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => null);

        var actual =
            await _sut.GetCheckedUserAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<CancellationToken>());

        actual.IsSucceed.Should().BeFalse();
        actual.ErrorCode.Should().Be(ErrorCode.InvalidCredentials);
        actual.Payload.Should().BeNull();
        
        _encryptionProviderMock.Verify(x =>
            x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<SaltAndHash>()), Times.Never);
    }

    [Fact]
    public async Task ReturnValidUser_WhenValidDataProvided()
    {
        var expectedUserFromRepository = new User
        {
            Id = 1,
            Email = "benotar@email.com",
            Username = "benotar_",
            PasswordSalt = [1, 2, 3],
            PasswordHash = [4, 5, 6]
        };
        
        var requestEmail = "benotar@email.com";
        var requestPassword = "VeryStrongPassword";
        var clt = CancellationToken.None;
        
        var validUserPasswordSaltAndHash = new SaltAndHash(expectedUserFromRepository.PasswordSalt,
            expectedUserFromRepository.PasswordHash);

        var expectedUser = new UserModel
        {
            Id = expectedUserFromRepository.Id,
            Email = expectedUserFromRepository.Email,
            Username = expectedUserFromRepository.Username
        };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(requestEmail, clt))
            .ReturnsAsync(expectedUserFromRepository);

        _encryptionProviderMock
            .Setup(x => x.VerifyPasswordHash(requestPassword, validUserPasswordSaltAndHash))
            .Returns(true);

        var actual = await _sut.GetCheckedUserAsync(requestEmail, requestPassword, clt);

        actual.IsSucceed.Should().BeTrue();
        actual.Payload.Should().BeEquivalentTo(expectedUser);
        actual.ErrorCode.Should().BeNull();
    }

    [Fact]
    public async Task ReturnInvalidCredentialsErrorCode_WhenPasswordNotMatch()
    {
        var expectedUserFromRepository = new User
        {
            Email = "benotar@email.com",
            Username = "benotar_",
            PasswordSalt = [1, 2, 3],
            PasswordHash = [4, 5, 6]
        };
        
        var requestEmail = "benotar@email.com";
        var requestPassword = "VeryStrongPassword";
        var clt = CancellationToken.None;
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(requestEmail, clt))
            .ReturnsAsync(expectedUserFromRepository);

        var validUserPasswordSaltAndHash = new SaltAndHash(expectedUserFromRepository.PasswordSalt,
            expectedUserFromRepository.PasswordHash);
        
        _encryptionProviderMock
            .Setup(x => x.VerifyPasswordHash(requestPassword, validUserPasswordSaltAndHash))
            .Returns(false);

        var actual = await _sut.GetCheckedUserAsync(requestEmail, requestPassword, clt);

        actual.IsSucceed.Should().BeFalse();
        actual.Payload.Should().BeNull();
        actual.ErrorCode.Should().Be(ErrorCode.InvalidCredentials);
    }
}