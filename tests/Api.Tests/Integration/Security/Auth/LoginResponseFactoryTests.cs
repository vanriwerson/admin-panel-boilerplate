using Api.Dtos;
using Api.Models;
using Api.Security.Auth;
using Api.Security.Jwt;
using Api.Security.RefreshTokens;
using Api.Interfaces.Repositories;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Moq;

namespace Api.Tests.Integration.Security.Auth;

[Collection("PostgreSql")]
public class LoginResponseFactoryTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public LoginResponseFactoryTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
        const string secretKey =
            "test-secret-key-1234567890abcdefghijklmnopqr";

        Environment.SetEnvironmentVariable(
            "JWT_SECRET_KEY",
            secretKey);

        JwtServices.Initialize(
            new Api.Settings.JwtSettings
            {
                SecretKey = secretKey
            });
    }

    #region CreateResponseAsync

    [Fact]
    public async Task CreateResponseAsync_Should_Create_Login_Response()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var resource = new SystemResource
        {
            Name = "USERS",
            ExhibitionName = "Usuários"
        };

        context.SystemResources.Add(resource);

        await context.SaveChangesAsync();

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hash"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        context.AccessPermissions.Add(
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = resource.Id
            });

        await context.SaveChangesAsync();

        await context.Entry(user)
            .Collection(x => x.AccessPermissions)
            .LoadAsync();

        foreach (var permission in user.AccessPermissions)
        {
            await context.Entry(permission)
                .Reference(x => x.SystemResource)
                .LoadAsync();
        }

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x =>
                x.AddAsync(
                    It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var refreshService =
            new RefreshTokenServices(
                refreshRepositoryMock.Object,
                context);

        var factory =
            new LoginResponseFactory(
                refreshService);

        var result =
            await factory.CreateResponseAsync(user);

        result.Should().NotBeNull();

        result.Id.Should().Be(user.Id);

        result.Username.Should().Be(
            user.Username);

        result.FullName.Should().Be(
            user.FullName);

        result.Token.Should().NotBeNullOrWhiteSpace();

        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateResponseAsync_Should_Return_User_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var usersResource =
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários"
            };

        var reportsResource =
            new SystemResource
            {
                Name = "REPORTS",
                ExhibitionName = "Relatórios"
            };

        context.SystemResources.AddRange(
            usersResource,
            reportsResource);

        await context.SaveChangesAsync();

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hash"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        context.AccessPermissions.AddRange(
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = usersResource.Id
            },
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = reportsResource.Id
            });

        await context.SaveChangesAsync();

        await context.Entry(user)
            .Collection(x => x.AccessPermissions)
            .LoadAsync();

        foreach (var permission in user.AccessPermissions)
        {
            await context.Entry(permission)
                .Reference(x => x.SystemResource)
                .LoadAsync();
        }

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x =>
                x.AddAsync(
                    It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var refreshService =
            new RefreshTokenServices(
                refreshRepositoryMock.Object,
                context);

        var factory =
            new LoginResponseFactory(
                refreshService);

        var result =
            await factory.CreateResponseAsync(user);

        result.Permissions
            .Should()
            .HaveCount(2);

        result.Permissions
            .Select(x => x.Name)
            .Should()
            .BeEquivalentTo(
                "USERS",
                "REPORTS");
    }

    [Fact]
    public async Task CreateResponseAsync_Should_Ignore_Inactive_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var activeResource =
            new SystemResource
            {
                Name = "USERS",
                ExhibitionName = "Usuários",
                Active = true
            };

        var inactiveResource =
            new SystemResource
            {
                Name = "REPORTS",
                ExhibitionName = "Relatórios",
                Active = false
            };

        context.SystemResources.AddRange(
            activeResource,
            inactiveResource);

        await context.SaveChangesAsync();

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hash"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        context.AccessPermissions.AddRange(
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = activeResource.Id
            },
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = inactiveResource.Id
            });

        await context.SaveChangesAsync();

        await context.Entry(user)
            .Collection(x => x.AccessPermissions)
            .LoadAsync();

        foreach (var permission in user.AccessPermissions)
        {
            await context.Entry(permission)
                .Reference(x => x.SystemResource)
                .LoadAsync();
        }

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x =>
                x.AddAsync(
                    It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var refreshService =
            new RefreshTokenServices(
                refreshRepositoryMock.Object,
                context);

        var factory =
            new LoginResponseFactory(
                refreshService);

        var result =
            await factory.CreateResponseAsync(user);

        result.Permissions
            .Should()
            .ContainSingle(x => x.Name == "USERS");
    }

    [Fact]
    public async Task CreateResponseAsync_Should_Generate_RefreshToken()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            Password = "hash"
        };

        var refreshRepositoryMock =
            new Mock<IRefreshTokenRepository>();

        refreshRepositoryMock
            .Setup(x =>
                x.AddAsync(
                    It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var refreshService =
            new RefreshTokenServices(
                refreshRepositoryMock.Object,
                context);

        var factory =
            new LoginResponseFactory(
                refreshService);

        var result =
            await factory.CreateResponseAsync(user);

        refreshRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<RefreshToken>(token => token.UserId == user.Id)),
            Times.Once);

        result.RefreshToken
            .Should()
            .NotBeNullOrWhiteSpace();
    }

    #endregion
}