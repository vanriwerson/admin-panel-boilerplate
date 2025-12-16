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
    public class CreateUser
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<SystemResource> _resourceRepo;
        private readonly ApiDbContext _context;
        private readonly CreateAccessPermission _createAccessPermission;
        private readonly CreateSystemLog _createSystemLog;

        public CreateUser(
            IGenericRepository<User> userRepo,
            IGenericRepository<SystemResource> resourceRepo,
            CreateAccessPermission createAccessPermission,
            ApiDbContext context,
            CreateSystemLog createSystemLog)
        {
            _userRepo = userRepo;
            _resourceRepo = resourceRepo;
            _context = context;
            _createAccessPermission = createAccessPermission;
            _createSystemLog = createSystemLog;
        }

        public async Task<UserReadDto> ExecuteAsync(UserCreateDto dto)
        {
            ValidateEntity.HasExpectedProperties<UserCreateDto>(dto);
            ValidateEntity.HasExpectedValues<UserCreateDto>(dto);

            if (dto.Permissions == null || !dto.Permissions.Any())
                throw new AppException("O usuário precisa ter pelo menos uma permissão.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    Password = PasswordHashing.Generate(dto.Password),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                if (await _userRepo.Query().AnyAsync(u => u.Email == user.Email || u.Username == user.Username))
                    throw new AppException("Email ou Username já cadastrado.", (int)HttpStatusCode.Conflict);

                await _userRepo.CreateAsync(user);
                await _context.SaveChangesAsync();

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

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Serializar o payload para o log (sem senha por segurança)
                var logData = new
                {
                    dto.Username,
                    dto.Email,
                    dto.FullName,
                    dto.Permissions
                };
                var payloadData = System.Text.Json.JsonSerializer.Serialize(logData, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });
                await _createSystemLog.ExecuteAsync(LogActionDescribe.Create("User", user.Id), data: payloadData);

                var createdUser = await _userRepo.Query()
                    .Include(u => u.AccessPermissions)
                    .ThenInclude(ap => ap.SystemResource)
                    .FirstAsync(u => u.Id == user.Id);

                return UserMapper.MapToUserReadDto(createdUser);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
