namespace Api.Auditing;

public class AuditLogService
{
    private readonly IGenericRepository<SystemLog> _repository;
    private readonly CurrentUserContext _currentUser;

    public AuditLogService(
        IGenericRepository<SystemLog> repository,
        CurrentUserContext currentUser
    )
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task RegisterAsync(
        string action,
        int? userId = null,
        string? payload = null
    )
    {
        var resolvedUserId = userId ?? _currentUser.GetId();

        var log = new SystemLog
        {
            UserId = resolvedUserId,
            Action = action,
            UsedPayload = payload,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(log);
    }
}
