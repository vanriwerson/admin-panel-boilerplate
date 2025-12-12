using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Api.Helpers
{
    public static class JsonWebToken
    {
        private static readonly string _secretKey = EnvLoader.GetEnv("JWT_SECRET_KEY");

        public static string Create(Claim[] claims, int expireMinutes = 480)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal Decode(string token, bool validateLifetime = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);
        }

        public static bool Verify(string token)
        {
            try
            {
                Decode(token);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int[] GetPermissionIds(ClaimsPrincipal principal)
        {
            return principal.Claims
                .Where(c => c.Type == "permission")
                .Select(c => int.Parse(c.Value))
                .ToArray();
        }
    }
}
