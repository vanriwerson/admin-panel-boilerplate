using Api.Auditing;
using Api.Data;
using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Services.AccessPermissions;
using Api.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Users.Orchestrators;

public class UpdateUserWithAccessGranted
{
    private readonly ApiDbContext _context;
    private readonly UpdateUser _updateUser;
    private readonly IUserRepository _userRepository;
    private readonly IAccessPermissionRepository _accessPermissionRepository;
    private readonly CreateAccessPermissions _createAccessPermissions;
    private readonly SystemLogService _logService;

    public UpdateUserWithAccessGranted(
        ApiDbContext context,
        UpdateUser updateUser,
        IUserRepository userRepository,
        IAccessPermissionRepository accessPermissionRepository,
        CreateAccessPermissions createAccessPermissions,
        SystemLogService logService)
    {
        _context = context;
        _updateUser = updateUser;
        _userRepository = userRepository;
        _accessPermissionRepository = accessPermissionRepository;
        _createAccessPermissions = createAccessPermissions;
        _logService = logService;
    }

    public async Task<UserReadDto> ExecuteAsync(UserUpdateDto dto)
    {
        Guard.AgainstNonPositiveInt(dto.Id, nameof(dto.Id));

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var prevUser = await _userRepository.GetByIdAsync(dto.Id)
                ?? throw new AppException("Usuário não encontrado.");

            // snapshot do estado anterior (antes de qualquer alteração)
            var prevState = new
            {
                prevUser.Id,
                prevUser.Username,
                prevUser.Email,
                prevUser.FullName,
                Permissions = prevUser.AccessPermissions
                    .Select(p => p.SystemResourceId)
                    .ToList()
            };

            var updatedUser = await _updateUser.ExecuteAsync(dto);

            if (dto.PermissionIds != null)
            {
                await _accessPermissionRepository.RemoveByUserIdAsync(updatedUser.Id);

                if (dto.PermissionIds.Any())
                {
                    await _createAccessPermissions.ExecuteAsync(
                        updatedUser.Id,
                        dto.PermissionIds
                    );
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _logService.RegisterAsync(
                action: SystemLogActionFactory.Update("User", updatedUser.Id),
                data: new SystemLogDataDto
                {
                    Type = "update",
                    PrevState = prevState,
                    CurrState = dto
                }
            );

            var resultUser = await _userRepository.GetByIdAsync(updatedUser.Id)
                ?? throw new AppException("Usuário atualizado não encontrado.");

            return new UserReadDto
            {
                Id = resultUser.Id,
                Username = resultUser.Username,
                Email = resultUser.Email,
                FullName = resultUser.FullName,
                Permissions = resultUser.AccessPermissions
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
