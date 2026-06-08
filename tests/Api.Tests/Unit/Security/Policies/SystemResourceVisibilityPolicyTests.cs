using System.Security.Claims;
using Api.Models;
using Api.Security.Jwt;
using Api.Security.Permissions;
using Api.Security.Policies;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Security.Policies;

public class SystemResourceVisibilityPolicyTests
{
    #region ApplyToQuery

    [Fact]
    public void ApplyToQuery_Should_Return_Empty_When_Not_Authenticated()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: false);

        var resources =
            CreateResources()
                .AsQueryable();

        // Act
        var result =
            policy.ApplyToQuery(resources);

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

        var resources =
            CreateResources()
                .AsQueryable();

        // Act
        var result =
            policy.ApplyToQuery(resources);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public void ApplyToQuery_Should_Hide_Root_Resource_For_NonRoot_User()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true);

        var resources =
            CreateResources()
                .AsQueryable();

        // Act
        var result =
            policy.ApplyToQuery(resources)
                .ToList();

        // Assert
        result.Should().HaveCount(2);

        result.Should()
            .NotContain(
                x => x.Id == BasePermissions.ROOT);
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

        // Act
        var result =
            policy.CanAccess(
                CreateRootResource());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAccess_Should_Return_True_When_User_Is_Root()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true,
                permissions: [BasePermissions.ROOT]);

        // Act
        var result =
            policy.CanAccess(
                CreateRootResource());

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAccess_Should_Return_False_For_Root_Resource_When_User_Is_Not_Root()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true);

        // Act
        var result =
            policy.CanAccess(
                CreateRootResource());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAccess_Should_Return_True_For_NonRoot_Resource()
    {
        // Arrange
        var policy =
            CreatePolicy(
                authenticated: true);

        // Act
        var result =
            policy.CanAccess(
                new SystemResource
                {
                    Id = BasePermissions.USERS
                });

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    private static SystemResourceVisibilityPolicy CreatePolicy(
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

        return new SystemResourceVisibilityPolicy(
            new CurrentUserContext(accessor));
    }

    private static List<SystemResource> CreateResources()
        =>
        [
            CreateRootResource(),
            new()
            {
                Id = BasePermissions.USERS,
                Name = "USERS"
            },
            new()
            {
                Id = BasePermissions.REPORTS,
                Name = "REPORTS"
            }
        ];

    private static SystemResource CreateRootResource()
        =>
            new()
            {
                Id = BasePermissions.ROOT,
                Name = "ROOT"
            };
}