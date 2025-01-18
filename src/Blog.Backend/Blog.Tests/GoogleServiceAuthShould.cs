using Blog.Application.Interfaces.Services;
using Blog.Application.Interfaces.UnitOfWork;
using Blog.Application.Models;
using Blog.Application.Services;
using Blog.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Blog.Tests;

public class GoogleServiceAuthShould
{
    private readonly GoogleService _sut;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly  Mock<IUserService> _userServiceMock;
    
    private readonly CancellationToken _clt;
    private readonly string _email;
    private readonly string _name;
    private readonly string _pictureUrl;


    public GoogleServiceAuthShould()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userServiceMock = new Mock<IUserService>();
        _sut = new GoogleService(_unitOfWorkMock.Object);

        _email = "some@email.com";
        _name = "some name";
        _pictureUrl = "some picture url";
        _clt = CancellationToken.None;
    }

    [Fact]
    public async Task ReturnUser_WhenUserExists()
    {
        var expectedUserFromRepo = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordSalt = [1, 2, 3],
            PasswordHash = [4, 5, 6],
            CreatedAt = DateTimeOffset.Now,
            UpdatedAt = DateTimeOffset.Now,
            ProfilePictureUrl = "url",
            Username = "username",
        };

        var expectedUserModelResult = new UserModel
        {
            Id = expectedUserFromRepo.Id,
            Email = expectedUserFromRepo.Email,
            Username = expectedUserFromRepo.Username,
            ProfilePictureUrl = expectedUserFromRepo.ProfilePictureUrl,
        };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetUserByEmailAsync(_email, _clt))
            .ReturnsAsync(expectedUserFromRepo);

        var actual = await _sut.FindOrCreateGoogleUserAsync(_email, _name, _pictureUrl, _clt);

        actual.IsSucceed.Should().BeTrue();
        actual.ErrorCode.Should().BeNull();
        actual.Payload.Should().Be(expectedUserModelResult);
    }

    // public async Task CreateAndReturnUser_WhenUserDoesNotExist()
    // {
    //     
    // }
}