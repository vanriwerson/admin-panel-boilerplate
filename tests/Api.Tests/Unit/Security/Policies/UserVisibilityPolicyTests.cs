using System.Security.Claims;
using Api.Interfaces.Security.Policies;
using Api.Models;
using Api.Security.Jwt;
using Api.Security.Permissions;
using Api.Security.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Security.Policies;

public class UserVisibilityPolicyTests
{
    #region ApplyToQuery

    [Fact]
    public void ApplyToQuery_Should_Return_Empty_When_User_Is_Not_Authenticated()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: false);

        var users =
            CreateUsers()
                .AsQueryable();

        // Act
        var result =
            policy.ApplyToQuery(users);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ApplyToQuery_Should_Return_All_When_User_Is_Root()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true,
                permissions: [BasePermissions.ROOT]);

        var users =
            CreateUsers()
                .AsQueryable();

        // Act
        var result =
            policy.ApplyToQuery(users);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyToQuery_Should_Hide_Root_Users_When_Current_User_Is_Not_Root()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true);

        var users =
            CreateUsers()
                .AsQueryable();

        // Act
        var result =
            policy.ApplyToQuery(users)
                .ToList();

        // Assert
        result.Should().ContainSingle();

        result[0]
            .Username
            .Should()
            .Be("normal");
    }

    #endregion

    #region CanAccess

    [Fact]
    public void CanAccess_Should_Return_False_When_Not_Authenticated()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: false);

        var user =
            CreateNormalUser();

        // Act
        var result =
            policy.CanAccess(user);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAccess_Should_Return_True_When_Current_User_Is_Root()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true,
                permissions: [BasePermissions.ROOT]);

        var user =
            CreateRootUser();

        // Act
        var result =
            policy.CanAccess(user);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAccess_Should_Return_False_For_Root_User_When_Current_User_Is_Not_Root()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true);

        var rootUser =
            CreateRootUser();

        // Act
        var result =
            policy.CanAccess(rootUser);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAccess_Should_Return_True_For_Normal_User()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true);

        var user =
            CreateNormalUser();

        // Act
        var result =
            policy.CanAccess(user);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    private static IUserVisibilityPolicy CreatePolicy(
        bool authenticated,
        params int[] permissions)
    {
        var claims =
            new List<Claim>();

        if (authenticated)
        {
            claims.Add(
                new Claim("id", "1"));

            claims.Add(
                new Claim("username", "tester"));

            claims.AddRange(
                permissions.Select(
                    permission =>
                        new Claim(
                            "permission",
                            permission.ToString())));
        }

        var identity =
            authenticated
                ? new ClaimsIdentity(
                    claims,
                    "Test")
                : new ClaimsIdentity();

        var principal =
            new ClaimsPrincipal(identity);

        var context =
            new DefaultHttpContext();

        context.User = principal;

        var accessor =
            new HttpContextAccessor
            {
                HttpContext = context
            };

        return new UserVisibilityPolicy(
            new CurrentUserContext(accessor));
    }

    private static List<User> CreateUsers()
        =>
        [
            CreateNormalUser(),
            CreateRootUser()
        ];

    private static User CreateNormalUser()
        =>
            new()
            {
                Username = "normal",
                AccessPermissions =
                [
                    new AccessPermission
                    {
                        SystemResourceId =
                            BasePermissions.USERS
                    }
                ]
            };

    private static User CreateRootUser()
        =>
            new()
            {
                Username = "root",
                AccessPermissions =
                [
                    new AccessPermission
                    {
                        SystemResourceId =
                            BasePermissions.ROOT
                    }
                ]
            };
}