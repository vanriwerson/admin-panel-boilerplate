using System.Security.Claims;
using System.Text;
using Api.Middlewares;
using Api.Security.Jwt;
using Api.Security.Permissions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Middlewares;

public class ValidateUserPermissionsTests
{
    public ValidateUserPermissionsTests()
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

    private static string GenerateToken(
        params int[] permissions)
    {
        var claims = new List<Claim>
        {
            new("id", "1"),
            new("username", "admin")
        };

        claims.AddRange(
            permissions.Select(p =>
                new Claim("permission", p.ToString())));

        return JwtServices.Create(claims);
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_Auth_Routes()
    {
        var nextCalled = false;

        var middleware = new ValidateUserPermissions(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Path = "/auth/login";

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_401_When_Header_Is_Missing()
    {
        var middleware =
            new ValidateUserPermissions(_ =>
                Task.CompletedTask);

        var context = new DefaultHttpContext();
        context.Request.Path = "/users";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_Root_User()
    {
        var nextCalled = false;

        var middleware = new ValidateUserPermissions(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var token = GenerateToken(BasePermissions.ROOT);

        var context = new DefaultHttpContext();
        context.Request.Path = "/users";

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Return_403_When_User_Has_No_Required_Permission()
    {
        var middleware =
            new ValidateUserPermissions(_ =>
                Task.CompletedTask);

        var token = GenerateToken(
            BasePermissions.REPORTS);

        var context = new DefaultHttpContext();
        context.Request.Path = "/users";

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        await middleware.InvokeAsync(context);

        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_When_User_Has_Required_Permission()
    {
        var nextCalled = false;

        var middleware = new ValidateUserPermissions(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var token = GenerateToken(
            BasePermissions.USERS);

        var context = new DefaultHttpContext();
        context.Request.Path = "/users";

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_Should_Block_Creation_With_Root_Permission()
    {
        var middleware =
            new ValidateUserPermissions(_ =>
                Task.CompletedTask);

        var token = GenerateToken(
            BasePermissions.USERS);

        var context = new DefaultHttpContext();

        context.Request.Path = "/users";
        context.Request.Method = "POST";

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        var body =
            """
            {
                "permissions":[1]
            }
            """;

        context.Request.Body =
            new MemoryStream(
                Encoding.UTF8.GetBytes(body));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_Should_Block_Creation_With_Resources_Permission()
    {
        var middleware =
            new ValidateUserPermissions(_ =>
                Task.CompletedTask);

        var token = GenerateToken(
            BasePermissions.USERS);

        var context = new DefaultHttpContext();

        context.Request.Path = "/users";
        context.Request.Method = "POST";

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        var body =
            """
            {
                "permissions":[3]
            }
            """;

        context.Request.Body =
            new MemoryStream(
                Encoding.UTF8.GetBytes(body));

        await middleware.InvokeAsync(context);

        context.Response.StatusCode
            .Should()
            .Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_Should_Allow_Creation_With_Normal_Permissions()
    {
        var nextCalled = false;

        var middleware = new ValidateUserPermissions(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var token = GenerateToken(
            BasePermissions.USERS);

        var context = new DefaultHttpContext();

        context.Request.Path = "/users";
        context.Request.Method = "POST";

        context.Request.Headers.Authorization =
            $"Bearer {token}";

        var body =
            """
            {
                "permissions":[4]
            }
            """;

        context.Request.Body =
            new MemoryStream(
                Encoding.UTF8.GetBytes(body));

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
    }
}