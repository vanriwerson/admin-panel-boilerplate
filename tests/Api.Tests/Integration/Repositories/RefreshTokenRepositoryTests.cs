using Api.Models;
using Api.Repositories;
using Api.Security.RefreshTokens;
using Api.Tests.Integration.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Integration.Repositories;

[Collection("PostgreSql")]
public class RefreshTokenRepositoryTests
{
    private readonly PostgreSqlTestFixture _fixture;

    public RefreshTokenRepositoryTests(
        PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddAsync_Should_Add_RefreshToken()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var token = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = "hash",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        await repository.AddAsync(token);
        await context.SaveChangesAsync();

        var stored =
            await context.RefreshTokens
                .FirstOrDefaultAsync();

        stored.Should().NotBeNull();
    }

    [Fact]
    public async Task FindByHashAsync_Should_Return_Token()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var hash =
            HashToken.Compute("raw-token");

        context.RefreshTokens.Add(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = hash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            });

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        var result =
            await repository.FindByHashAsync(hash);

        result.Should().NotBeNull();
        result!.TokenHash.Should().Be(hash);
    }

    [Fact]
    public async Task RevokeAsync_Should_Revoke_Token()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var token = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = "hash",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        context.RefreshTokens.Add(token);
        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        await repository.RevokeAsync(
            token,
            DateTime.UtcNow);

        await context.SaveChangesAsync();

        token.IsRevoked.Should().BeTrue();
        token.RevokedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RevokeAllForUserAsync_Should_Revoke_All_User_Tokens()
    {
        await using var context =
            _fixture.CreateContext();

        await DatabaseCleaner.ResetAsync(context);

        var user = new User
        {
            Username = "user",
            Email = "user@email.com",
            FullName = "User",
            Password = "hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.RefreshTokens.AddRange(
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = "hash1",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            },
            new RefreshToken
            {
                UserId = user.Id,
                TokenHash = "hash2",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            });

        await context.SaveChangesAsync();

        var repository =
            new RefreshTokenRepository(context);

        await repository.RevokeAllForUserAsync(user.Id);

        await context.SaveChangesAsync();

        var tokens =
            await context.RefreshTokens.ToListAsync();

        tokens.Should()
            .OnlyContain(t => t.IsRevoked);
    }
}