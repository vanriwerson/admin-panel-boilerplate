using Api.Models;
using Api.Repositories;
using Api.Security.RefreshTokens;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Security.RefreshTokens;

[Collection("PostgreSql")]
public class RefreshTokenServicesTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public RefreshTokenServicesTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region GenerateAsync

    [Fact]
    public async Task GenerateAsync_Should_Create_RefreshToken()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var result =
            await service.GenerateAsync(user);

        result.rawToken
            .Should()
            .NotBeNullOrWhiteSpace();

        result.entity
            .Should()
            .NotBeNull();

        var token =
            await context.RefreshTokens
                .FirstOrDefaultAsync();

        token.Should().NotBeNull();

        token!.UserId.Should().Be(user.Id);

        token.IsRevoked.Should().BeFalse();

        token.ExpiresAt.Should()
            .BeAfter(DateTime.UtcNow);

        HashToken.Verify(
            result.rawToken,
            token.TokenHash)
            .Should()
            .BeTrue();
    }

    #endregion

    #region ValidateAsync

    [Fact]
    public async Task ValidateAsync_Should_Return_Token_When_Valid()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var generated =
            await service.GenerateAsync(user);

        var result =
            await service.ValidateAsync(
                generated.rawToken);

        result.Should().NotBeNull();

        result!.Id.Should()
            .Be(generated.entity.Id);
    }

    [Fact]
    public async Task ValidateAsync_Should_Return_Null_When_Token_Does_Not_Exist()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var result =
            await service.ValidateAsync(
                "token-inexistente");

        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateAsync_Should_Return_Null_When_Token_Is_Revoked()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var generated =
            await service.GenerateAsync(user);

        await service.RevokeAsync(
            generated.entity);

        var result =
            await service.ValidateAsync(
                generated.rawToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateAsync_Should_Return_Null_When_Token_Is_Expired()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var rawToken =
            Guid.NewGuid().ToString("N");

        var refreshToken =
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = HashToken.Compute(rawToken),
                ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

        context.RefreshTokens.Add(
            refreshToken);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var result =
            await service.ValidateAsync(
                rawToken);

        result.Should().BeNull();
    }

    #endregion

    #region RevokeAsync

    [Fact]
    public async Task RevokeAsync_Should_Revoke_Token()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var generated =
            await service.GenerateAsync(user);

        await service.RevokeAsync(
            generated.entity);

        var token =
            await context.RefreshTokens
                .FirstAsync();

        token.IsRevoked.Should().BeTrue();

        token.RevokedAt.Should()
            .NotBeNull();
    }

    #endregion

    #region RevokeAllForUserAsync

    [Fact]
    public async Task RevokeAllForUserAsync_Should_Revoke_All_User_Tokens()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        await service.GenerateAsync(user);
        await service.GenerateAsync(user);
        await service.GenerateAsync(user);

        await service.RevokeAllForUserAsync(
            user.Id);

        var tokens =
            await context.RefreshTokens
                .ToListAsync();

        tokens.Should()
            .OnlyContain(x => x.IsRevoked);
    }

    #endregion

    #region RotateAsync

    [Fact]
    public async Task RotateAsync_Should_Revoke_Old_Token_And_Create_New_One()
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

        context.Users.Add(user);

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var original =
            await service.GenerateAsync(user);

        original.entity.User = user;

        var rotated =
            await service.RotateAsync(
                original.entity);

        var oldToken =
            await context.RefreshTokens
                .FirstAsync(
                    x => x.Id == original.entity.Id);

        oldToken.IsRevoked.Should().BeTrue();

        rotated.rawToken
            .Should()
            .NotBeNullOrWhiteSpace();

        rotated.newEntity.Id
            .Should()
            .NotBe(original.entity.Id);

        var count =
            await context.RefreshTokens
                .CountAsync();

        count.Should().Be(2);
    }

    [Fact]
    public async Task RotateAsync_Should_Throw_When_User_Is_Not_Loaded()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var repository =
            new RefreshTokenRepository(context);

        var service =
            new RefreshTokenServices(
                repository,
                context);

        var token =
            new RefreshToken
            {
                UserId = 1
            };

        Func<Task> act =
            () => service.RotateAsync(token);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(
                "User must be loaded");
    }

    #endregion
}