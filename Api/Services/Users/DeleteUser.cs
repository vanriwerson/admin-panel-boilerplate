using Api.Auditing;
using Api.Auditing.Services;
using Api.Helpers;
using Api.Interfaces.Repositories;
using Api.Interfaces.Auditing.Services;

namespace Api.Services.Users;

public class DeleteUser
{
    private readonly IUserRepository _userRepository;
    private readonly ICreateSystemLog _createSystemLog;

    public DeleteUser(
        IUserRepository userRepository,
        ICreateSystemLog createSystemLog)
    {
        _userRepository = userRepository;
        _createSystemLog = createSystemLog;
    }

    public async Task<bool> ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id);

        var deleted = await _userRepository.SoftDeleteAsync(id);

        if (deleted)
        {
            await _createSystemLog.ExecuteAsync(
                action: SystemLogActionFactory.Delete("User", id)
            );
        }

        return deleted;
    }
}
