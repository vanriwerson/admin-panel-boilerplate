using System.Security.Claims;
using Api.Security.Jwt;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Security.Jwt;

public class JwtAuthenticationTests
{
    public JwtAuthenticationTests()
    {
        Environment.SetEnvironmentVariable(
            "JWT_SECRET_KEY",
            "unit-tests-secret-key-with-32-chars");
    }

    [Fact]
    public async Task InvokeAsync_Should_Call_Next_When_Header_Is_Missing()
    {
        // Arrange
        var nextCalled = false;

        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new JwtAuthentication(next);

        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();

        context.User.Claims.Should().BeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_Should_Ignore_NonBearer_Header()
    {
        // Arrange
        var nextCalled = false;

        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new JwtAuthentication(next);

        var context = new DefaultHttpContext();

        context.Request.Headers.Authorization =
            "Basic abc123";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();

        context.User.Claims.Should().BeEmpty();
    }

    [Fact]
    public async Task InvokeAsync_Should_Populate_User_When_Jwt_Is_Valid()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("id", "1"),
            new Claim("email", "user@email.com"),
            new Claim("username", "user")
        };

        var token = JwtServices.Create(claims);

        var nextCalled = false;

        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new JwtAuthentication(next);

        var context = new DefaultHttpContext();

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();

        context.User.Should().NotBeNull();

        context.User.FindFirst("id")?.Value
            .Should().Be("1");

        context.User.FindFirst("email")?.Value
            .Should().Be("user@email.com");

        context.User.FindFirst("username")?.Value
            .Should().Be("user");
    }

    [Fact]
    public async Task InvokeAsync_Should_Ignore_Malformed_Token()
    {
        // Arrange
        var nextCalled = false;

        RequestDelegate next = context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new JwtAuthentication(next);

        var context = new DefaultHttpContext();

        context.Request.Headers.Authorization =
            "Bearer token-malformado";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();

        context.User.Claims.Should().BeEmpty();
    }
}