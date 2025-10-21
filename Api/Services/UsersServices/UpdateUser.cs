using System.Net;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Api.Services.AccessPermissionsServices;
using Api.Services.SystemLogsServices;

namespace Api.Services.UsersServices
{
    public class UpdateUser
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly ApiDbContext _context;
        private readonly DeleteAccessPermissions _deleteAccessPermissions;
        private readonly CreateAccessPermission _createAccessPermission;
        private readonly CreateSystemLog _createSystemLog;

        public UpdateUser(
            IGenericRepository<User> userRepo,
            DeleteAccessPermissions deleteAccessPermissions,
            CreateAccessPermission createAccessPermission,
            ApiDbContext context,
            CreateSystemLog createSystemLog)
        {
            _userRepo = userRepo;
            _deleteAccessPermissions = deleteAccessPermissions;
            _createAccessPermission = createAccessPermission;
            _context = context;
            _createSystemLog = createSystemLog;
        }

        public async Task<UserReadDto?> ExecuteAsync(int id, UserUpdateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepo.Query()
                    .Include(u => u.AccessPermissions)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return null;

                if (!string.IsNullOrWhiteSpace(dto.Email) || !string.IsNullOrWhiteSpace(dto.Username))
                {
                    bool isDuplicate = await _userRepo.Query()
                        .AnyAsync(u =>
                            u.Id != id &&
                            ((dto.Email != null && u.Email == dto.Email) ||
                             (dto.Username != null && u.Username == dto.Username)));

                    if (isDuplicate)
                        throw new AppException("Email ou Username já cadastrado por outro usuário.", (int)HttpStatusCode.Conflict);
                }

                user.Username = dto.Username ?? user.Username;
                user.Email = dto.Email ?? user.Email;
                user.FullName = dto.FullName ?? user.FullName;
                user.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                    user.Password = PasswordHashing.Generate(dto.Password);

                await _deleteAccessPermissions.ExecuteAsync(user.Id);

                if (dto.Permissions != null && dto.Permissions.Any())
                {
                    foreach (var resourceId in dto.Permissions)
                    {
                        var permissionDto = new AccessPermissionCreateDto
                        {
                            UserId = user.Id,
                            SystemResourceId = resourceId
                        };

                        await _createAccessPermission.ExecuteAsync(permissionDto);
                    }
                }

                await _userRepo.UpdateAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _createSystemLog.ExecuteAsync(
                    userId: user.Id,
                    action: LogActionDescribe.Update("User", user.Id)
                );

                var updatedUser = await _userRepo.Query()
                    .Include(u => u.AccessPermissions)
                    .ThenInclude(ap => ap.SystemResource)
                    .FirstAsync(u => u.Id == user.Id);

                return UserMapper.MapToUserReadDto(updatedUser);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
