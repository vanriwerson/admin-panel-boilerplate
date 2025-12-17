using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Models;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        public async Task<LoginResponseDto?> ExchangeExternalTokenAsync(string externalToken)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = JsonWebToken.Decode(externalToken);
            }
            catch
            {
                return null;
            }

            var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var loginClaim = principal.Claims.FirstOrDefault(c => c.Type == "login")?.Value;
            var usernameClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var username = loginClaim ?? usernameClaim;

            if (emailClaim == null && username == null)
                return null;

            var user = await _context.Users
                .Include(u => u.AccessPermissions)
                .ThenInclude(ap => ap.SystemResource)
                .FirstOrDefaultAsync(u => u.Email == emailClaim || u.Username == username);

            if (user == null)
                return null;

            var claims = DefaultJWTClaims.Generate(user);
            var token = JsonWebToken.Create(claims);

            await _createSystemLog.ExecuteAsync(
                userId: user.Id,
                action: LogActionDescribe.ExternalLogin(user.Username)
            );

            var allowedResources = (user.AccessPermissions ?? new List<AccessPermission>())
                .Where(ap => ap.SystemResource != null && ap.SystemResource.Active)
                .Select(ap => new SystemResourceOptionDto
                {
                    Id = ap.SystemResource!.Id,
                    Name = ap.SystemResource.Name,
                    ExhibitionName = ap.SystemResource.ExhibitionName
                })
                .ToList();

            return new LoginResponseDto
            {
                Token = token,
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Permissions = allowedResources
            };
        }
    }
}
