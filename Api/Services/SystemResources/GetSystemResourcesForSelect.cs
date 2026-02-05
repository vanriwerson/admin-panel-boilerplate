using Api.Dtos;
using Api.Interfaces.Repositories;

namespace Api.Services.SystemResources;

public class GetSystemResourcesForSelect
{
    private readonly ISystemResourceRepository _repository;

    public GetSystemResourcesForSelect(ISystemResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SystemResourceSelectDto>> ExecuteAsync()
    {
        var resources = await _repository.GetForSelectAsync();

        return resources.Select(r => new SystemResourceSelectDto
        {
            Id = r.Id,
            ExhibitionName = r.ExhibitionName
        });
    }
}
