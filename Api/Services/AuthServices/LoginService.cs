using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Helpers;

namespace Api.Services.AuthServices
{
    public class LoginService
    {
        private readonly ApiDbContext _context;
        private readonly string _jwtSecret;

        public LoginService(ApiDbContext context)
        {
            _context = context;
            _jwtSecret = EnvLoader.GetEnv("JWT_SECRET");
        }

        public async Task<string?> LoginAsync(string identifier, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);

            if (user == null)
                return null;

            if (!PasswordHashing.Verify(password, user.Password))
                return null;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = JsonWebToken.Create(_jwtSecret, claims, expireMinutes: 120);
            return token;
        }
    }
}
