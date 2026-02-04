using Api.Dtos;
using Api.Helpers;
using Api.Interfaces.Repositories;

namespace Api.Auditing.Services;

public class GetSystemLogById
{
    private readonly ISystemLogRepository _repository;

    public GetSystemLogById(ISystemLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<SystemLogReadDto> ExecuteAsync(int id)
    {
        Guard.AgainstNonPositiveInt(id, nameof(id));

        return await _repository.GetByIdAsync(id)
            ?? throw new AppException("Log não encontrado.");
    }
}
