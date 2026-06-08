using System.Security.Claims;
using Api.Security.Jwt;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Unit.Security.Jwt;

public class CurrentUserContextTests
{
    #region IsAuthenticated

    [Fact]
    public void IsAuthenticated_Should_Return_True_When_User_Is_Authenticated()
    {
        // Arrange

        var context =
            CreateContext(
                authenticated: true);

        // Act

        var result =
            context.IsAuthenticated;

        // Assert

        result.Should().BeTrue();
    }

    [Fact]
    public void IsAuthenticated_Should_Return_False_When_User_Is_Not_Authenticated()
    {
        // Arrange

        var context =
            CreateContext(
                authenticated: false);

        // Act

        var result =
            context.IsAuthenticated;

        // Assert

        result.Should().BeFalse();
    }

    #endregion

    #region GetId

    [Fact]
    public void GetId_Should_Return_User_Id_When_Claim_Exists()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim("id", "15")
                ]);

        // Act

        var result =
            context.GetId();

        // Assert

        result.Should().Be(15);
    }

    [Fact]
    public void GetId_Should_Return_Null_When_Claim_Does_Not_Exist()
    {
        // Arrange

        var context =
            CreateContext();

        // Act

        var result =
            context.GetId();

        // Assert

        result.Should().BeNull();
    }

    [Fact]
    public void GetId_Should_Return_Null_When_Claim_Is_Invalid()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim("id", "abc")
                ]);

        // Act

        var result =
            context.GetId();

        // Assert

        result.Should().BeNull();
    }

    #endregion

    #region GetUsername

    [Fact]
    public void GetUsername_Should_Return_Username_Claim()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim(
                        "username",
                        "bruno")
                ]);

        // Act

        var result =
            context.GetUsername();

        // Assert

        result.Should().Be("bruno");
    }

    [Fact]
    public void GetUsername_Should_Return_Name_Claim_When_Username_Is_Missing()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim(
                        ClaimTypes.Name,
                        "Bruno Silva")
                ]);

        // Act

        var result =
            context.GetUsername();

        // Assert

        result.Should().Be(
            "Bruno Silva");
    }

    [Fact]
    public void GetUsername_Should_Return_Null_When_No_Claim_Exists()
    {
        // Arrange

        var context =
            CreateContext();

        // Act

        var result =
            context.GetUsername();

        // Assert

        result.Should().BeNull();
    }

    #endregion

    #region GetIpAddress

    [Fact]
    public void GetIpAddress_Should_Return_Ip_Address()
    {
        // Arrange

        var context =
            CreateContext(
                ipAddress:
                "192.168.0.10");

        // Act

        var result =
            context.GetIpAddress();

        // Assert

        result.Should().Be(
            "192.168.0.10");
    }

    [Fact]
    public void GetIpAddress_Should_Return_Null_When_Ip_Is_Not_Defined()
    {
        // Arrange

        var context =
            CreateContext();

        // Act

        var result =
            context.GetIpAddress();

        // Assert

        result.Should().BeNull();
    }

    #endregion

    #region HasPermission

    [Fact]
    public void HasPermission_Should_Return_True_When_User_Has_Permission()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim(
                        "permission",
                        "100")
                ]);

        // Act

        var result =
            context.HasPermission(100);

        // Assert

        result.Should().BeTrue();
    }

    [Fact]
    public void HasPermission_Should_Return_False_When_User_Does_Not_Have_Permission()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim(
                        "permission",
                        "200")
                ]);

        // Act

        var result =
            context.HasPermission(100);

        // Assert

        result.Should().BeFalse();
    }

    #endregion

    #region IsRoot

    [Fact]
    public void IsRoot_Should_Return_True_When_Root_Permission_Exists()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim(
                        "permission",
                        "1")
                ]);

        // Act

        var result =
            context.IsRoot();

        // Assert

        result.Should().BeTrue();
    }

    [Fact]
    public void IsRoot_Should_Return_False_When_Root_Permission_Does_Not_Exist()
    {
        // Arrange

        var context =
            CreateContext(
                claims:
                [
                    new Claim(
                        "permission",
                        "2")
                ]);

        // Act

        var result =
            context.IsRoot();

        // Assert

        result.Should().BeFalse();
    }

    #endregion

    private static CurrentUserContext CreateContext(
        bool authenticated = true,
        IEnumerable<Claim>? claims = null,
        string? ipAddress = null)
    {
        claims ??= [];

        var identity =
            authenticated
                ? new ClaimsIdentity(
                    claims,
                    "Test")
                : new ClaimsIdentity();

        var principal =
            new ClaimsPrincipal(
                identity);

        var httpContext =
            new DefaultHttpContext();

        httpContext.User =
            principal;

        if (ipAddress != null)
        {
            httpContext.Connection.RemoteIpAddress =
                System.Net.IPAddress.Parse(
                    ipAddress);
        }

        var accessor =
            new HttpContextAccessor
            {
                HttpContext = httpContext
            };

        return new CurrentUserContext(
            accessor);
    }
}