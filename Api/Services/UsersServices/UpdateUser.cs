using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Api.Services;
using Api.Validations;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Services.UsersServices
{
    public class UpdateUser
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<SystemResource> _resourceRepo;
        private readonly ApiDbContext _context;
        private readonly DeleteAccessPermissions _deleteAccessPermissions;
        private readonly CreateAccessPermission _createAccessPermission;
        private readonly CreateSystemLog _createSystemLog;

        public UpdateUser(
            IGenericRepository<User> userRepo,
            IGenericRepository<SystemResource> resourceRepo,
            DeleteAccessPermissions deleteAccessPermissions,
            CreateAccessPermission createAccessPermission,
            ApiDbContext context,
            CreateSystemLog createSystemLog)
        {
            _userRepo = userRepo;
            _resourceRepo = resourceRepo;
            _deleteAccessPermissions = deleteAccessPermissions;
            _createAccessPermission = createAccessPermission;
            _context = context;
            _createSystemLog = createSystemLog;
        }

        public async Task<UserReadDto?> ExecuteAsync(int id, UserUpdateDto dto)
        {
            ValidateEntity.HasExpectedProperties<UserUpdateDto>(dto);
            ValidateEntity.HasExpectedValues<UserUpdateDto>(dto);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepo.Query()
                    .Include(u => u.AccessPermissions)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                    return null;

                // Capturar o estado anterior para o log
                var prevState = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FullName,
                    Permissions = user.AccessPermissions.Select(ap => ap.SystemResourceId).ToList()
                };
                var prevStateJson = System.Text.Json.JsonSerializer.Serialize(prevState, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });

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
                        await ValidateEntity.EnsureEntityExistsAsync(_resourceRepo, resourceId, "Recurso do sistema");

                        var permissionDto = new AccessPermissionCreateDto
                        {
                            UserId = user.Id,
                            SystemResourceId = resourceId
                        };
                        await _createAccessPermission.ExecuteAsync(permissionDto);
                    }
                }
                else
                {
                    throw new AppException("O usuário precisa ter pelo menos uma permissão.");
                }

                await _userRepo.UpdateAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                await _createSystemLog.ExecuteAsync(LogActionDescribe.Update("User", user.Id), data: prevStateJson);

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
