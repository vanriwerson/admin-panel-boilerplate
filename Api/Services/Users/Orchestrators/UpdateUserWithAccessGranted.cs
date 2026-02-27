using Api.Auditing;
using Api.Auditing.Services;
using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Interfaces.Repositories;
using Api.Mappers;
using Api.Middlewares;
using Api.Security.Policies;
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
    private readonly CreateSystemLog _createSystemLog;
    private readonly AccessPermissionPolicy _accessPermissionPolicy;

    public UpdateUserWithAccessGranted(
        ApiDbContext context,
        UpdateUser updateUser,
        IUserRepository userRepository,
        IAccessPermissionRepository accessPermissionRepository,
        CreateAccessPermissions createAccessPermissions,
        CreateSystemLog createSystemLog,
        AccessPermissionPolicy accessPermissionPolicy)
    {
        _context = context;
        _updateUser = updateUser;
        _userRepository = userRepository;
        _accessPermissionRepository = accessPermissionRepository;
        _createAccessPermissions = createAccessPermissions;
        _createSystemLog = createSystemLog;
        _accessPermissionPolicy = accessPermissionPolicy;
    }

    public async Task<UserReadDto> ExecuteAsync(UserUpdateDto dto)
    {
        Guard.AgainstNonPositiveInt(dto.Id);

        if (dto.PermissionIds != null)
        {
            _accessPermissionPolicy.EnsureCanGrant(dto.PermissionIds);
        }

        object prevState;
        object currState;

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByIdAsync(dto.Id)
                ?? throw new AppException("Usuário não encontrado.");

            prevState = new
            {
                user.Username,
                user.Email,
                user.FullName,
                Permissions = user.AccessPermissions
                    .Select(p => p.SystemResourceId)
                    .ToList()
            };

            var updatedUser = await _updateUser.ExecuteAsync(dto);
            await _context.SaveChangesAsync();

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

                await _context.SaveChangesAsync();
            }

            currState = new
            {
                updatedUser.Username,
                updatedUser.Email,
                updatedUser.FullName,
                Permissions = dto.PermissionIds
            };

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        await _createSystemLog.ExecuteAsync(
            action: SystemLogActionFactory.Update("User", dto.Id),
            data: new SystemLogDataDto
            {
                Type = "update",
                PrevState = prevState,
                CurrState = currState
            }
        );

        var resultUser = await _userRepository.GetByIdAsync(dto.Id)
            ?? throw new AppException("Usuário atualizado não encontrado.");

        return UserMapper.MapToUserReadDto(resultUser);
    }
}
