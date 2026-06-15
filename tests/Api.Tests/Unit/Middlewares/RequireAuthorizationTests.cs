using System.Security.Claims;
using Api.Middlewares;
using Api.Security.Jwt;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Middlewares;

public class RequireAuthorizationTests
{
    public RequireAuthorizationTests()
    {
        Environment.SetEnvironmentVariable(
            "JWT_SECRET_KEY",
            "unit-tests-secret-key-with-32-chars");

        JwtServices.Initialize(
            new Api.Settings.JwtSettings
            {
                SecretKey = "unit-tests-secret-key-with-32-chars"
            });
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_Auth_Routes()
    {
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new RequireAuthorization(next);

        var context = new DefaultHttpContext();
        context.Request.Path = "/auth/login";

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_Password_Routes()
    {
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new RequireAuthorization(next);

        var context = new DefaultHttpContext();
        context.Request.Path = "/password/reset";

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_401_When_Header_Is_Missing()
    {
        var middleware = new RequireAuthorization(_ =>
            Task.CompletedTask);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_401_When_Token_Is_Invalid()
    {
        var middleware = new RequireAuthorization(_ =>
            Task.CompletedTask);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";

        context.Request.Headers.Authorization =
            "Bearer invalid-token";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_Should_Call_Next_When_Token_Is_Valid()
    {
        var nextCalled = false;

        RequestDelegate next = _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new RequireAuthorization(next);

        var token = JwtServices.Create(
            new[]
            {
                new Claim("id", "1"),
                new Claim("email", "user@email.com")
            });

        var context = new DefaultHttpContext();

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }
}