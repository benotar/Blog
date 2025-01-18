using Blog.Application.Common;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Application.Services;
using Blog.Domain.Entities;
using FluentAssertions;
using Moq;
using Moq.Language.Flow;

namespace Blog.Tests;

public class UserServiceCreateGoogleUserShould
{
    private readonly ISetup<IMomentProvider, DateTimeOffset> _nowSetup;
    private readonly Mock<IEncryptionProvider> _mockEncryptionProvider;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    private readonly UserService _sut;

    public UserServiceCreateGoogleUserShould()
    {
        var mockMomentProvider = new Mock<IMomentProvider>();
        _nowSetup = mockMomentProvider.Setup(momentProvider => momentProvider.DateTimeOffsetUtcNow);

        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _mockEncryptionProvider = new Mock<IEncryptionProvider>();

        _sut = new UserService(mockMomentProvider.Object, _mockUnitOfWork.Object,
            _mockEncryptionProvider.Object);
    }

    [Fact]
    public async Task ReturnCreatedUser_WhenDataIsProvided()
    {
        var email = "email@email.com";
        var username = "some username";
        var pictureUrl = "some picture url";
        var saltAndHash = new SaltAndHash([1, 2, 3], [4, 5, 6]);
        var now = new DateTimeOffset(2025, 01, 16, 18, 00, 00, TimeSpan.FromHours(2));
        var ctl = CancellationToken.None;
        var password = "some password";

        var expectedCreatedUser = new User
        {
            Email = email,
            Username = username,
            PasswordSalt = saltAndHash.Salt,
            PasswordHash = saltAndHash.Hash,
            ProfilePictureUrl = pictureUrl,
            CreatedAt = now,
        };

        var expectedReturnedUser = new UserModel
        {
            Email = expectedCreatedUser.Email,
            Username = expectedCreatedUser.Username,
            ProfilePictureUrl = expectedCreatedUser.ProfilePictureUrl
        };

        _nowSetup.Returns(now);
        _mockEncryptionProvider.Setup(ep => ep.HashPassword(password)).Returns(saltAndHash);
        _mockUnitOfWork.Setup(uow => uow.UserRepository.Add(expectedCreatedUser));

        var actual = await _sut.CreateGoogleAsync(username, email, password, pictureUrl, ctl);

        actual.Payload.Should().BeEquivalentTo(expectedReturnedUser,
            options => options.Excluding(p => p.Id));
        actual.IsSucceed.Should().BeTrue();
        actual.ErrorCode.Should().BeNull();
    }
}