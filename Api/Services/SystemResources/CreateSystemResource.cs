using Api.Auditing.Services;
using Api.Data;
using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Middlewares;
using Api.Models;
using Api.Validations;

namespace Api.Services.SystemResources;

public class CreateSystemResource
{
    private readonly ISystemResourceRepository _repository;
    private readonly ApiDbContext _context;

    public CreateSystemResource(
      ISystemResourceRepository repository,
      ApiDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<SystemResourceReadDto> ExecuteAsync(SystemResourceCreateDto dto)
    {
        Guard.AgainstNull(dto, "Payload inválido para criação de SystemResource");

        if (await _repository.ExistsByNameAsync(dto.Name))
            throw new AppException("Já existe um recurso com esse nome.");

        var resource = new SystemResource
        {
            Name = dto.Name,
            ExhibitionName = dto.ExhibitionName
        };

        await _repository.CreateAsync(resource);
        await _context.SaveChangesAsync();

        return new SystemResourceReadDto
        {
            Id = resource.Id,
            Name = resource.Name,
            ExhibitionName = resource.ExhibitionName
        };
    }
}
