using System.Security.Claims;
using Api.Models;
using Api.Security.Jwt;
using Api.Security.Permissions;
using FluentAssertions;

namespace Api.Tests.Unit.Security.Jwt;

public class JwtClaimsFactoryTests
{
    #region FromUser

    [Fact]
    public void FromUser_Should_Create_Basic_Claims()
    {
        // Arrange

        var user = new User
        {
            Id = 10,
            Username = "bruno",
            FullName = "Bruno Silva",
            Email = "bruno@email.com"
        };

        // Act

        var claims =
            JwtClaimsFactory
                .FromUser(user)
                .ToList();

        // Assert

        claims.Should()
            .Contain(c =>
                c.Type == "id" &&
                c.Value == "10");

        claims.Should()
            .Contain(c =>
                c.Type == ClaimTypes.Name &&
                c.Value == "bruno");

        claims.Should()
            .Contain(c =>
                c.Type == "username" &&
                c.Value == "bruno");

        claims.Should()
            .Contain(c =>
                c.Type == "fullName" &&
                c.Value == "Bruno Silva");

        claims.Should()
            .Contain(c =>
                c.Type == "email" &&
                c.Value == "bruno@email.com");
    }

    [Fact]
    public void FromUser_Should_Create_Permission_Claims()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",

            AccessPermissions =
            [
                new AccessPermission
                {
                    SystemResource = new SystemResource
                    {
                        Id = BasePermissions.USERS,
                        Name = "USERS"
                    }
                },

                new AccessPermission
                {
                    SystemResource = new SystemResource
                    {
                        Id = BasePermissions.REPORTS,
                        Name = "REPORTS"
                    }
                }
            ]
        };

        // Act

        var claims =
            JwtClaimsFactory
                .FromUser(user)
                .ToList();

        // Assert

        claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .Should()
            .BeEquivalentTo(
                BasePermissions.USERS.ToString(),
                BasePermissions.REPORTS.ToString());
    }

    [Fact]
    public void FromUser_Should_Not_Create_Permission_Claims_When_User_Has_No_Permissions()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",
            AccessPermissions = []
        };

        // Act

        var claims =
            JwtClaimsFactory
                .FromUser(user)
                .ToList();

        // Assert

        claims.Should()
            .NotContain(c => c.Type == "permission");
    }

    [Fact]
    public void FromUser_Should_Ignore_Permissions_Without_SystemResource()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            Username = "bruno",
            Email = "bruno@email.com",
            FullName = "Bruno Silva",

            AccessPermissions =
            [
                new AccessPermission
                {
                    SystemResource = null!
                }
            ]
        };

        // Act

        var claims =
            JwtClaimsFactory
                .FromUser(user)
                .ToList();

        // Assert

        claims.Should()
            .NotContain(c => c.Type == "permission");
    }

    [Fact]
    public void FromUser_Should_Throw_When_User_Is_Null()
    {
        // Arrange

        User? user = null;

        // Act

        Action act =
            () => JwtClaimsFactory
                .FromUser(user!)
                .ToList();

        // Assert

        act.Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromUser_Should_Create_Root_Permission_Claim()
    {
        // Arrange

        var user = new User
        {
            Id = 1,
            Username = "root",
            FullName = "Root User",
            Email = "root@email.com",

            AccessPermissions =
            [
                new AccessPermission
                {
                    SystemResource = new SystemResource
                    {
                        Id = BasePermissions.ROOT,
                        Name = "ROOT"
                    }
                }
            ]
        };

        // Act

        var claims =
            JwtClaimsFactory
                .FromUser(user)
                .ToList();

        // Assert

        claims.Should()
            .Contain(c =>
                c.Type == "permission" &&
                c.Value == BasePermissions.ROOT.ToString());
    }

    #endregion
}