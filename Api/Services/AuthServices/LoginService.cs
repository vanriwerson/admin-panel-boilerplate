using Api.Auditing;
using Api.Auditing.Services;
using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Models;
using Api.Security.Jwt;
using Api.Security.Passwords;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services.AuthServices
{
    public class LoginService
    {
        private readonly ApiDbContext _context;
        private readonly CreateSystemLog _createSystemLog;

        public LoginService(ApiDbContext context, CreateSystemLog createSystemLog)
        {
            _context = context;
            _createSystemLog = createSystemLog;
        }

        public async Task<LoginResponseDto?> LoginAsync(string identifier, string password)
        {
            var user = await _context.Users
                .Include(u => u.AccessPermissions)
                    .ThenInclude(ap => ap.SystemResource)
                .FirstOrDefaultAsync(u => u.Email == identifier || u.Username == identifier);

            if (user == null || !PasswordHash.Verify(password, user.Password))
                return null;

            var claims = JwtClaimsFactory.FromUser(user);
            var token = JwtService.Create(claims);

            await _createSystemLog.ExecuteAsync(
                userId: user.Id,
                action: SystemLogActionFactory.Login(user.Username)
            );

            var allowedResources = (user.AccessPermissions ?? new List<AccessPermission>())
                .Where(ap => ap.SystemResource != null && ap.SystemResource.Active)
                .Select(ap => new SystemResourceSelectDto
                {
                    Id = ap.SystemResource!.Id,
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
