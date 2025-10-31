using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Api.Services;
using Api.Validations;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Services.SystemResourcesServices
{
  public class UpdateSystemResource
  {
    private readonly IGenericRepository<SystemResource> _repo;
    private readonly CreateSystemLog _createSystemLog;

    public UpdateSystemResource(IGenericRepository<SystemResource> repo, CreateSystemLog createSystemLog)
    {
      _repo = repo;
      _createSystemLog = createSystemLog;
    }

    public async Task<SystemResourceReadDto?> ExecuteAsync(int id, SystemResourceUpdateDto dto)
    {
      ValidateEntity.HasExpectedProperties<SystemResourceUpdateDto>(dto);
      ValidateEntity.HasExpectedValues<SystemResourceUpdateDto>(dto);

      var resource = await _repo.GetByIdAsync(id);
      if (resource == null)
        return null;

      if (!string.IsNullOrWhiteSpace(dto.Name) || !string.IsNullOrWhiteSpace(dto.ExhibitionName))
      {
        bool isDuplicate = await _repo.Query()
            .AnyAsync(r =>
                r.Id != id &&
                ((dto.Name != null && r.Name == dto.Name) ||
                 (dto.ExhibitionName != null && r.ExhibitionName == dto.ExhibitionName))
            );

        if (isDuplicate)
          throw new AppException("Já existe um recurso com o mesmo nome ou nome de exibição.", (int)HttpStatusCode.Conflict);
      }

      resource.Name = dto.Name ?? resource.Name;
      resource.ExhibitionName = dto.ExhibitionName ?? resource.ExhibitionName;
      resource.Active = dto.Active ?? resource.Active;
      resource.UpdatedAt = DateTime.UtcNow;

      var updated = await _repo.UpdateAsync(resource);

      await _createSystemLog.ExecuteAsync(
          action: LogActionDescribe.Update("SystemResource", updated.Id)
      );

      return new SystemResourceReadDto
      {
        Id = updated.Id,
        Name = updated.Name,
        ExhibitionName = updated.ExhibitionName,
        Active = updated.Active,
        CreatedAt = updated.CreatedAt,
        UpdatedAt = updated.UpdatedAt
      };
    }
  }
}
