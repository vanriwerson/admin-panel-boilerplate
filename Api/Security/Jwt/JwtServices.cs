using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace Api.Security.Jwt;

public static class JwtServices
{
    private static readonly string SecretKey =
        EnvLoader.GetEnv("JWT_SECRET_KEY");

    // NOTE: during tests we set default lifetime to 1 minute.  In
    // production you can override this by passing a different value or
    // reading from configuration.
    public static string Create(
        IEnumerable<Claim> claims,
        int expireMinutes = 1
    )
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(SecretKey)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static ClaimsPrincipal Validate(
        string token,
        bool validateLifetime = true
    )
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SecretKey)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.Zero
        };

        return new JwtSecurityTokenHandler()
            .ValidateToken(token, parameters, out _);
    }

    public static bool IsValid(string token)
    {
        try
        {
            Validate(token);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
