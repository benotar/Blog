using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;

namespace Blog.Tests;

public class UserServiceCreateAsyncShould
{
    private readonly UserService _sut;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IEncryptionProvider> _mockEncryptionProvider;
    private readonly ISetup<IMomentProvider, DateTimeOffset> _nowSetup;

    public UserServiceCreateAsyncShould()
    {
        var mockMomentProvider = new Mock<IMomentProvider>();
        _nowSetup = mockMomentProvider.Setup(momentProvider => momentProvider.DateTimeOffsetUtcNow);

        _mockEncryptionProvider = new Mock<IEncryptionProvider>();
        
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _sut = new UserService(mockMomentProvider.Object, _mockUnitOfWork.Object,
            _mockEncryptionProvider.Object);
    }

    [Fact]
    public async Task ReturnFalse_WhenUserAlreadyExists()
    {
        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.AnyByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var actual = await _sut.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

        actual.Should().BeEquivalentTo(Result<None>.Error(ErrorCode.UserAlreadyExists));
        _mockUnitOfWork.Verify(
            uow => uow.UserRepository.AnyByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.UserRepository.Add(It.IsAny<User>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReturnSuccess_WhenValidDataIsProvided()
    {
        var username = "username";
        var email = "email@email.com";
        var password = "123456";
        var saltAndHash = new SaltAndHash([1, 2, 3], [4, 5, 6]);
        var now = new DateTimeOffset(2025, 01, 06, 21, 51, 00, TimeSpan.FromHours(2));
        var cancellationToken = CancellationToken.None;
        
        _mockUnitOfWork
            .Setup(uow => uow.UserRepository.AnyByEmailAsync(email, cancellationToken))
            .ReturnsAsync(false);
        _nowSetup.Returns(now);
        _mockEncryptionProvider.Setup(ep => ep.HashPassword(password)).Returns(saltAndHash);
        
        var actual =
            await _sut.CreateAsync(username, email, password, cancellationToken);
        
        actual.Should().BeEquivalentTo(Result<None>.Success());
        
        _mockUnitOfWork
            .Verify(unitOfWork => unitOfWork.UserRepository
                .AnyByEmailAsync(email, cancellationToken), Times.Once);
        
        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.UserRepository
            .Add(It.Is<User>(user => 
                user.Username == username &&
                user.Email == email &&
                user.PasswordSalt == saltAndHash.Salt &&
                user.PasswordHash == saltAndHash.Hash &&
                user.CreatedAt == now)), Times.Once);
        
        _mockUnitOfWork.Verify(unitOfWork => unitOfWork
            .SaveChangesAsync(cancellationToken), Times.Once);
    }
}