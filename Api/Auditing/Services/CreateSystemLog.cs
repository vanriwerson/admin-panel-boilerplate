using Api.Interfaces.Repositories;
using Api.Models;
using Api.Security.Jwt;

namespace Api.Auditing.Services;

public class CreateSystemLog
{
    private readonly ISystemLogRepository _repository;
    private readonly CurrentUserContext _currentUser;

    public CreateSystemLog(
        ISystemLogRepository repository,
        CurrentUserContext currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(
        string action,
        object? data = null,
        int? userId = null)
    {
        var resolvedUserId = userId ?? _currentUser.GetId();

        var log = new SystemLog
        {
            UserId = resolvedUserId,
            Action = action,
            Data = data != null
                ? SystemLogDataSerializer.Serialize(data)
                : null,
            GeneratedBy = _currentUser.GetUsername(),
            IpAddress = _currentUser.GetIpAddress(),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(log);
    }
}
