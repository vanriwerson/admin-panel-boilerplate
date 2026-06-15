using System.Security.Claims;
using Api.Security.Jwt;

namespace Api.Tests.Integration.Infrastructure;

public static class TestJwtFactory
{
    static TestJwtFactory()
    {
        const string secretKey =
            "unit-tests-secret-key-with-32-chars";

        Environment.SetEnvironmentVariable(
            "JWT_SECRET_KEY",
            secretKey);

        JwtServices.Initialize(
            new Api.Settings.JwtSettings
            {
                SecretKey = secretKey
            });
    }

    public static string CreateRootToken(
        int userId = 1,
        string username = "root")
    {
        var claims = new List<Claim>
        {
            new("id", userId.ToString()),
            new(ClaimTypes.Name, username),
            new("username", username),
            new("permission", "1") // ROOT
        };

        return JwtServices.Create(claims);
    }

    public static string CreateUserToken(
        int userId,
        params int[] permissions)
    {
        var claims = new List<Claim>
        {
            new("id", userId.ToString()),
            new(ClaimTypes.Name, $"user-{userId}")
        };

        claims.AddRange(
            permissions.Select(
                p => new Claim("permission", p.ToString())
            ));

        return JwtServices.Create(claims);
    }
}