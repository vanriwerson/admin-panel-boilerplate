using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Api.Services.AccessPermissionsServices;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Services.UsersServices
{
    public class CreateUser
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly ApiDbContext _context;
        private readonly CreateAccessPermission _createAccessPermission;

        public CreateUser(
            IGenericRepository<User> userRepo,
            CreateAccessPermission createAccessPermission,
            ApiDbContext context)
        {
            _userRepo = userRepo;
            _context = context;
            _createAccessPermission = createAccessPermission;
        }

        public async Task<UserReadDto> ExecuteAsync(UserCreateDto dto)
        {
            if (!ValidateEntity.HasValidProperties<UserCreateDto>(dto))
                throw new AppException("A requisição não possui os campos esperados.", (int)HttpStatusCode.BadRequest);

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

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

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
