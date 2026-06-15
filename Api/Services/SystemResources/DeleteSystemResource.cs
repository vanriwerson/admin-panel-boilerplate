using Api.Auditing;
using Api.Interfaces.Auditing.Services;
using Api.Interfaces.Repositories;
using Api.Middlewares;

namespace Api.Services.SystemResources;

public class DeleteSystemResource
{
    private readonly ISystemResourceRepository _repository;
    private readonly ICreateSystemLog _createSystemLog;

    public DeleteSystemResource(
        ISystemResourceRepository repository,
        ICreateSystemLog createSystemLog)
    {
        _repository = repository;
        _createSystemLog = createSystemLog;
    }

    public async Task ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id);

        var deleted = await _repository.SoftDeleteAsync(id);

        if (!deleted)
            throw new AppException("Recurso não encontrado.");

        await _createSystemLog.ExecuteAsync(
            action: SystemLogActionFactory.Delete("SystemResource", id)
        );
    }
}
