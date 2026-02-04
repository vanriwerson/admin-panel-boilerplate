using Api.Auditing;
using Api.Helpers;
using Api.Interfaces.Repositories;

namespace Api.Services.Users;

public class DeleteUser
{
    private readonly IUserRepository _userRepository;
    private readonly SystemLogService _systemLog;

    public DeleteUser(
        IUserRepository userRepository,
        SystemLogService systemLog)
    {
        _userRepository = userRepository;
        _systemLog = systemLog;
    }

    public async Task<bool> ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id, nameof(id));

        var deleted = await _userRepository.SoftDeleteAsync(id);

        if (deleted)
        {
            await _systemLog.RegisterAsync(
                action: SystemLogActionFactory.Delete("User", id)
            );
        }

        return deleted;
    }
}
