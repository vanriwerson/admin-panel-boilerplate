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

        public LoginService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<string?> LoginAsync(string identifier, string password)
        {
            var user = await _context.Users
                .Include(u => u.AccessPermissions)
                .ThenInclude(ap => ap.SystemResource)
                .FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);

            if (user == null || !PasswordHashing.Verify(password, user.Password))
                return null;

            var claims = DefaultJWTClaims.Generate(user);

            var token = JsonWebToken.Create(claims, expireMinutes: 120);

            return token;
        }
    }
}
