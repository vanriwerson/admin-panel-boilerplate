using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Helpers;

namespace Api.Services.AuthServices
{
    public class ExternalTokenService
    {
        private readonly ApiDbContext _context;
        private readonly string _jwtSecret;

        public ExternalTokenService(ApiDbContext context)
        {
            _context = context;
            _jwtSecret = EnvLoader.GetEnv("JWT_SECRET_KEY");
        }

         public async Task<string?> ExchangeExternalTokenAsync(string externalToken)
        {
            ClaimsPrincipal principal;

            try
            {
                principal = JsonWebToken.Decode(externalToken, _jwtSecret);
            }
            catch
            {
                return null; // token externo inválido
            }

            var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var usernameClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (emailClaim == null && usernameClaim == null)
                return null;

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == emailClaim || u.Username == usernameClaim);

            if (user == null)
                return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            return JsonWebToken.Create(_jwtSecret, claims, expireMinutes: 120);
        }
    }
}
