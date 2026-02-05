using Api.Dtos;
using Api.Data;
using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Validations;

namespace Api.Services.SystemResources;

public class UpdateSystemResource
{
    private readonly ISystemResourceRepository _repository;
    private readonly ApiDbContext _context;

    public UpdateSystemResource(
      ISystemResourceRepository repository,
      ApiDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<SystemResourceReadDto> ExecuteAsync(int id, SystemResourceUpdateDto dto)
    {
        Guard.AgainstNonPositiveInt(id);
        Guard.AgainstMismatchedIds(id, dto.Id);

        var resource = await _repository.GetByIdAsync(id)
          ?? throw new AppException("Recurso não encontrado.");

        if (!string.IsNullOrWhiteSpace(dto.Name))
            resource.Name = dto.Name;

        if (!string.IsNullOrWhiteSpace(dto.ExhibitionName))
            resource.ExhibitionName = dto.ExhibitionName;

        await _repository.UpdateAsync(resource);
        await _context.SaveChangesAsync();

        return new SystemResourceReadDto
        {
            Id = resource.Id,
            Name = resource.Name,
            ExhibitionName = resource.ExhibitionName
        };
    }
}
