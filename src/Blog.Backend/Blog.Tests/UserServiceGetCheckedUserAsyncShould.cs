using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Application.Services;
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
        actual.Data.Should().BeNull();
        
        _encryptionProviderMock.Verify(x => 
            x.HashPassword(It.IsAny<string>()), Times.Never);
        
        _encryptionProviderMock.Verify(x => 
                x.VerifyPasswordHash(It.IsAny<string>(), It.IsAny<SaltAndHash>()), Times.Never);
    }

    [Fact]
    public async Task ReturnValidUser_WhenValidDataProvided()
    {
        var email = "benotar@email.co,";
        var password = "VeryStrongPassword";
        var clt = CancellationToken.None;
        var userSaltAndHash = new SaltAndHash([1, 2, 3], [4, 5, 6]);
        
        var expectedUser = new UserModel
        {
            Email = email
        };

        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(email, clt))
            .ReturnsAsync(expectedUser);
        
        _encryptionProviderMock
            .Setup(x => x.HashPassword(password))
            .Returns(userSaltAndHash);
        
        _encryptionProviderMock
            .Setup(x => x.VerifyPasswordHash(password, userSaltAndHash))
            .Returns(true);
        
        var actual = await _sut.GetCheckedUserAsync(email, password, clt);
        
        actual.IsSucceed.Should().BeTrue();
        actual.Data.Should().BeEquivalentTo(expectedUser);
        actual.ErrorCode.Should().BeNull();
    }
}