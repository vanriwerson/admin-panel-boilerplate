using System.Security.Claims;
using Api.Models;
using Api.Security.Auth;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Security.Auth;

[Collection("PostgreSql")]
public class AuthUserResolverTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public AuthUserResolverTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region FindByIdentifierAsync

    [Fact]
    public async Task FindByIdentifierAsync_Should_Return_User_When_Email_Exists()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "hash",
            FullName = "Bruno Silva"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByIdentifierAsync(
                "bruno@email.com");

        result.Should().NotBeNull();

        result!.Email.Should().Be(
            "bruno@email.com");
    }

    [Fact]
    public async Task FindByIdentifierAsync_Should_Return_User_When_Username_Exists()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "hash",
            FullName = "Bruno Silva"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByIdentifierAsync(
                "bruno");

        result.Should().NotBeNull();

        result!.Username.Should().Be(
            "bruno");
    }

    [Fact]
    public async Task FindByIdentifierAsync_Should_Return_Null_When_User_Does_Not_Exist()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByIdentifierAsync(
                "inexistente");

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindByIdentifierAsync_Should_Load_Permissions()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        await DatabaseSeeder.SeedPermissionsAsync(
            context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "hash",
            FullName = "Bruno Silva"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        context.AccessPermissions.Add(
            new AccessPermission
            {
                UserId = user.Id,
                SystemResourceId = 2
            });

        await context.SaveChangesAsync();

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByIdentifierAsync(
                "bruno");

        result.Should().NotBeNull();

        result!.AccessPermissions
            .Should()
            .HaveCount(1);

        result.AccessPermissions
            .First()
            .SystemResource
            .Should()
            .NotBeNull();
    }

    #endregion

    #region FindByExternalClaimsAsync

    [Fact]
    public async Task FindByExternalClaimsAsync_Should_Return_User_When_Email_Claim_Matches()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "hash",
            FullName = "Bruno Silva"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var principal =
            CreatePrincipal(
                new Claim(
                    ClaimTypes.Email,
                    "bruno@email.com"));

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByExternalClaimsAsync(
                principal);

        result.Should().NotBeNull();

        result!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task FindByExternalClaimsAsync_Should_Return_User_When_Login_Claim_Matches()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "hash",
            FullName = "Bruno Silva"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var principal =
            CreatePrincipal(
                new Claim(
                    "login",
                    "bruno"));

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByExternalClaimsAsync(
                principal);

        result.Should().NotBeNull();

        result!.Username.Should().Be(
            "bruno");
    }

    [Fact]
    public async Task FindByExternalClaimsAsync_Should_Return_User_When_Name_Claim_Matches()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "bruno",
            Email = "bruno@email.com",
            Password = "hash",
            FullName = "Bruno Silva"
        };

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var principal =
            CreatePrincipal(
                new Claim(
                    ClaimTypes.Name,
                    "bruno"));

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByExternalClaimsAsync(
                principal);

        result.Should().NotBeNull();

        result!.Username.Should().Be(
            "bruno");
    }

    [Fact]
    public async Task FindByExternalClaimsAsync_Should_Return_Null_When_No_Claims_Are_Present()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var principal =
            new ClaimsPrincipal(
                new ClaimsIdentity());

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByExternalClaimsAsync(
                principal);

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindByExternalClaimsAsync_Should_Return_Null_When_User_Is_Not_Found()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var principal =
            CreatePrincipal(
                new Claim(
                    ClaimTypes.Email,
                    "naoexiste@email.com"));

        var service =
            new AuthUserResolver(context);

        var result =
            await service.FindByExternalClaimsAsync(
                principal);

        result.Should().BeNull();
    }

    #endregion

    private static ClaimsPrincipal CreatePrincipal(
        params Claim[] claims)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                "Test"));
    }
}