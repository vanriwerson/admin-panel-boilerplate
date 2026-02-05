using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Validations;

namespace Api.Services.SystemResources;

public class DeleteSystemResource
{
    private readonly ISystemResourceRepository _repository;

    public DeleteSystemResource(ISystemResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id);

        var deleted = await _repository.SoftDeleteAsync(id);

        if (!deleted)
            throw new AppException("Recurso não encontrado.");
    }
}
