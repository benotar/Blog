using System.Linq.Expressions;
using Blog.Application.Common;
using Blog.Application.Interfaces.FactoryMethod;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.Repository;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models.Response.User;
using Blog.Application.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Blog.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly Mock<IMomentProvider> _momentProviderMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPaginationFactory<UserModel>> _paginationFactoryMock;
    private readonly Mock<IRepository<User>> _userRepositoryMock;

    public UserServiceTests()
    {
        _momentProviderMock = new Mock<IMomentProvider>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _paginationFactoryMock = new Mock<IPaginationFactory<UserModel>>();
        _userRepositoryMock = new Mock<IRepository<User>>();

        _unitOfWorkMock
            .Setup(u => u.GetRepository<User>())
            .Returns(_userRepositoryMock.Object);


        _sut = new UserService(_momentProviderMock.Object,
            _passwordHasherMock.Object,
            _unitOfWorkMock.Object,
            _paginationFactoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_UsernameAlreadyExists_ReturnsError()
    {
        // Arrange
        var ct = CancellationToken.None;

        _userRepositoryMock
            .Setup(repo => repo.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), ct))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync("existing_user", "email@test.com", "password", ct);

        // Assert
        result.IsSucceed.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.UsernameAlreadyExist);

        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), ct), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_EmailAlreadyExists_ReturnsError()
    {
        // Arrange
        var ct = CancellationToken.None;

        _userRepositoryMock
            .SetupSequence(repo => repo.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), ct))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync("some_user", "email@test.com", "password", ct);

        // Assert
        result.IsSucceed.Should().BeFalse();
        result.ErrorCode.Should().Be(ErrorCode.EmailAlreadyExist);

        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), ct), Times.Never);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(ct), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsSuccess()
    {
        var ct = CancellationToken.None;

        _userRepositoryMock
            .SetupSequence(repo => repo.AnyAsync(It.IsAny<Expression<Func<User, bool>>>(), ct))
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync("some_user", "email@test.com", "password", ct);

        // Assert
        result.IsSucceed.Should().BeTrue();
        result.ErrorCode.Should().BeNull();
        result.Payload.Should().Be(new None());

        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Once);

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), ct), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(ct), Times.Once);
    }
}
