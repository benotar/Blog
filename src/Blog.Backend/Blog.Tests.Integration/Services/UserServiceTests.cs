using Blog.Application.FactoryMethod;
using Blog.Application.Interfaces.Providers;
using Blog.Application.Models.Response.User;
using Blog.Application.Providers;
using Blog.Application.Services;
using Blog.Persistence;
using Blog.Persistence.UnitOfWork;
using Blog.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Tests.Integration.Services;

public class UserServiceTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly UserService _sut;

    public UserServiceTests(DatabaseFixture fixture)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;

        _dbContext = new AppDbContext(options);

        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var unitOfWork = new UnitOfWork(_dbContext, serviceProvider);
        var momentProvider = new MomentProvider();
        var passwordHasher = new BCryptPasswordHasher();
        var paginationFactory = new PaginationFactory<UserModel>();

        _sut = new UserService(momentProvider, passwordHasher, unitOfWork, paginationFactory);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task CreateAsync_ValidData_UserExistsInDatabase()
    {
        // Arrange
        var username = "ivan";
        var email = "ivan@test.com";
        var password = "Password123!";

        // Act
        var result = await _sut.CreateAsync(username, email, password);

        // Assert
        result.IsSucceed.Should().BeTrue();
        result.ErrorCode.Should().BeNull();

        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        userInDb.Should().NotBeNull();
        userInDb!.Username.Should().Be(username);
        userInDb.PasswordHash.Should().NotBeNullOrEmpty();

        userInDb.PasswordHash.Should().NotBe(password);
    }
}
