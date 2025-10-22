using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Api.Services.SystemLogsServices;
using System.Net;

namespace Api.Services.SystemResourcesServices
{
  public class CreateSystemResource
  {
    private readonly IGenericRepository<SystemResource> _repo;
    private readonly CreateSystemLog _createSystemLog;

    public CreateSystemResource(IGenericRepository<SystemResource> repo, CreateSystemLog createSystemLog)
    {
      _repo = repo;
      _createSystemLog = createSystemLog;
    }

    public async Task<SystemResourceReadDto> ExecuteAsync(SystemResourceCreateDto dto)
    {
      if (!ValidateEntity.HasValidProperties<SystemResourceCreateDto>(dto))
        throw new AppException("A requisição não possui os campos esperados.", (int)HttpStatusCode.BadRequest);

      var entity = new SystemResource
      {
        Name = dto.Name,
        ExhibitionName = dto.ExhibitionName,
        Active = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      var created = await _repo.CreateAsync(entity);

      await _createSystemLog.ExecuteAsync(
          action: LogActionDescribe.Create("SystemResource", entity.Id)
      );

      return new SystemResourceReadDto
      {
        Id = created.Id,
        Name = created.Name,
        ExhibitionName = created.ExhibitionName,
        Active = created.Active,
        CreatedAt = created.CreatedAt,
        UpdatedAt = created.UpdatedAt
      };
    }
  }
}
