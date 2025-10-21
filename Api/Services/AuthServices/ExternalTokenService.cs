using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Helpers;
using Api.Services.SystemLogsServices;

namespace Api.Services.AuthServices
{
    public class ExternalTokenService
    {
        private readonly ApiDbContext _context;
        private readonly CreateSystemLog _createSystemLog;

        public ExternalTokenService(ApiDbContext context, CreateSystemLog createSystemLog)
        {
            _context = context;
            _createSystemLog = createSystemLog;
        }

        public async Task<string?> ExchangeExternalTokenAsync(string externalToken)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = JsonWebToken.Decode(externalToken);
            }
            catch
            {
                return null; // token externo inválido
            }

            var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var usernameClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (emailClaim == null && usernameClaim == null)
                return null;

            var user = await _context.Users
                .Include(u => u.AccessPermissions)
                .ThenInclude(ap => ap.SystemResource)
                .FirstOrDefaultAsync(u =>
                    u.Email == emailClaim || u.Username == usernameClaim);

            if (user == null)
                return null;

            var claims = DefaultJWTClaims.Generate(user);
            var token = JsonWebToken.Create(claims, expireMinutes: 120);

            await _createSystemLog.ExecuteAsync(user.Id, LogActionDescribe.Login(user.Username));

            return token;
        }
    }
}
