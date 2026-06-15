using System.Security.Claims;
using Api.Middlewares;
using Api.Security.Jwt;
using Api.Security.Permissions;
using Api.Security.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Security.Policies;

public class AccessPermissionPolicyTests
{
    #region EnsureCanGrant

    [Fact]
    public void EnsureCanGrant_Should_Not_Throw_When_User_Is_Root()
    {
        // Arrange

        var policy =
            CreatePolicy(
                authenticated: true,
                permissions:
                [
                    BasePermissions.ROOT
                ]);

        // Act

        var act = () =>
            policy.EnsureCanGrant(
            [
                BasePermissions.ROOT,
                BasePermissions.USERS
            ]);

        // Assert

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanGrant_Should_Not_Throw_When_NonRoot_Does_Not_Grant_Root()
    {
        // Arrange

        var policy =
            CreatePolicy(
                authenticated: true,
                permissions:
                [
                    BasePermissions.USERS
                ]);

        // Act

        var act = () =>
            policy.EnsureCanGrant(
            [
                BasePermissions.USERS,
                BasePermissions.REPORTS
            ]);

        // Assert

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureCanGrant_Should_Throw_When_User_Is_Not_Authenticated()
    {
        // Arrange

        var policy =
            CreatePolicy(
                authenticated: false);

        // Act

        var act = () =>
            policy.EnsureCanGrant(
            [
                BasePermissions.USERS
            ]);

        // Assert

        var exception =
            act.Should()
                .Throw<AppException>()
                .Which;

        exception.Message.Should()
            .Be("Usuário não autenticado.");

        exception.StatusCode.Should()
            .Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void EnsureCanGrant_Should_Throw_When_NonRoot_Tries_To_Grant_Root()
    {
        // Arrange

        var policy =
            CreatePolicy(
                authenticated: true,
                permissions:
                [
                    BasePermissions.USERS
                ]);

        // Act

        var act = () =>
            policy.EnsureCanGrant(
            [
                BasePermissions.ROOT
            ]);

        // Assert

        var exception =
            act.Should()
                .Throw<AppException>()
                .Which;

        exception.Message.Should()
            .Be(
                "Apenas usuário ROOT pode conceder permissão ROOT.");

        exception.StatusCode.Should()
            .Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public void EnsureCanGrant_Should_Throw_When_NonRoot_Tries_To_Grant_Root_Among_Other_Permissions()
    {
        // Arrange

        var policy =
            CreatePolicy(
                authenticated: true,
                permissions:
                [
                    BasePermissions.USERS
                ]);

        // Act

        var act = () =>
            policy.EnsureCanGrant(
            [
                BasePermissions.USERS,
                BasePermissions.REPORTS,
                BasePermissions.ROOT
            ]);

        // Assert

        act.Should()
            .Throw<AppException>()
            .WithMessage(
                "Apenas usuário ROOT pode conceder permissão ROOT.");
    }

    [Fact]
    public void EnsureCanGrant_Should_Allow_Empty_Permission_List()
    {
        // Arrange

        var policy =
            CreatePolicy(
                authenticated: true,
                permissions:
                [
                    BasePermissions.USERS
                ]);

        // Act

        var act = () =>
            policy.EnsureCanGrant([]);

        // Assert

        act.Should().NotThrow();
    }

    #endregion

    private static AccessPermissionPolicy CreatePolicy(
        bool authenticated,
        params int[] permissions)
    {
        var claims =
            new List<Claim>
            {
                new("id", "1"),
                new("username", "bruno")
            };

        claims.AddRange(
            permissions.Select(
                permission =>
                    new Claim(
                        "permission",
                        permission.ToString())));

        var identity =
            authenticated
                ? new ClaimsIdentity(
                    claims,
                    "Test")
                : new ClaimsIdentity();

        var principal =
            new ClaimsPrincipal(identity);

        var httpContext =
            new DefaultHttpContext();

        httpContext.User =
            principal;

        var accessor =
            new HttpContextAccessor
            {
                HttpContext = httpContext
            };

        var currentUser =
            new CurrentUserContext(
                accessor);

        return new AccessPermissionPolicy(
            currentUser);
    }
}