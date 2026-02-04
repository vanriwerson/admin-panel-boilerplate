using Api.Auditing;
using Api.Auditing.Services;
using Api.Data;
using Api.Dtos;
using Api.Helpers;
using Api.Interfaces.Repositories;
using Api.Middlewares;
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
    private readonly CreateSystemLog _createSystemLog;

    public CreateUserWithAccessGranted(
        ApiDbContext context,
        CreateUser createUser,
        CreateAccessPermissions createAccessPermissions,
        IUserRepository userRepository,
        CreateSystemLog createSystemLog)
    {
        _context = context;
        _createUser = createUser;
        _createAccessPermissions = createAccessPermissions;
        _userRepository = userRepository;
        _createSystemLog = createSystemLog;
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

            await _createSystemLog.ExecuteAsync(
                userId: user.Id,
                action: SystemLogActionFactory.Create("User",user.Id)
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
                        ExhibitionName = ap.SystemResource.ExhibitionName
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
