using Api.Auditing;
using Api.Data;
using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Services.AccessPermissions;
using Api.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Users.Orchestrators;

public class CreateUserWithAccessGranted
{
    private readonly ApiDbContext _context;
    private readonly CreateUser _createUser;
    private readonly CreateAccessPermissions _createAccessPermissions;
    private readonly IUserRepository _userRepository;
    private readonly SystemLogService _logService;

    public CreateUserWithAccessGranted(
        ApiDbContext context,
        CreateUser createUser,
        CreateAccessPermissions createAccessPermissions,
        IUserRepository userRepository,
        SystemLogService logService)
    {
        _context = context;
        _createUser = createUser;
        _createAccessPermissions = createAccessPermissions;
        _userRepository = userRepository;
        _logService = logService;
    }

    public async Task<UserReadDto> ExecuteAsync(UserCreateDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _createUser.ExecuteAsync(dto);

            if (dto.PermissionIds.Any())
            {
                await _createAccessPermissions.ExecuteAsync(
                    user.Id,
                    dto.PermissionIds
                );
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _logService.RegisterAsync(
                action: SystemLogActionFactory.Create("User", user.Id),
                data: new SystemLogDataDto
                {
                    Type = "create",
                    Created = dto
                }
            );

            var createdUser = await _userRepository.GetByIdAsync(user.Id)
                ?? throw new AppException("Usuário recém-criado não encontrado.");

            return new UserReadDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                Permissions = createdUser.AccessPermissions
                    .Select(ap => new SystemResourceSelectDto
                    {
                        Id = ap.SystemResource.Id,
                        Name = ap.SystemResource.Name
                    })
                    .ToList()
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
