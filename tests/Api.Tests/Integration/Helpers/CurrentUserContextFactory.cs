using System.Security.Claims;
using Api.Security.Jwt;
using Microsoft.AspNetCore.Http;

namespace Api.Tests.Integration.Helpers;

public static class CurrentUserContextFactory
{
    public static CurrentUserContext Create(
        int userId,
        string username,
        params int[] permissions)
    {
        var claims = new List<Claim>
        {
            new("id", userId.ToString()),
            new("username", username)
        };

        claims.AddRange(
            permissions.Select(
                permission =>
                    new Claim(
                        "permission",
                        permission.ToString()
                    )
            )
        );

        var principal =
            new ClaimsPrincipal(
                new ClaimsIdentity(
                    claims,
                    authenticationType: "Test"
                )
            );

        var httpContext =
            new DefaultHttpContext();

        httpContext.User = principal;

        var accessor =
            new HttpContextAccessor
            {
                HttpContext = httpContext
            };

        return new CurrentUserContext(accessor);
    }
}