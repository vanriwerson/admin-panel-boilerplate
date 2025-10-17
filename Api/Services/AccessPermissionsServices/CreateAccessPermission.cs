using Api.Interfaces;
using Api.Models;
using Api.Dtos;
using Api.Middlewares;
using System.Net;

namespace Api.Services.AccessPermissionsServices
{
  public class CreateAccessPermission
  {
    private readonly IGenericRepository<AccessPermission> _repo;

    public CreateAccessPermission(IGenericRepository<AccessPermission> repo)
    {
      _repo = repo;
    }

    public async Task<AccessPermissionReadDto> ExecuteAsync(AccessPermissionCreateDto dto)
    {
      if (dto.UserId <= 0 || dto.SystemResourceId <= 0)
        throw new AppException("Usuário ou recurso inválido.", (int)HttpStatusCode.BadRequest);

      var permission = new AccessPermission
      {
        UserId = dto.UserId,
        SystemResourceId = dto.SystemResourceId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      var created = await _repo.CreateAsync(permission);

      return new AccessPermissionReadDto
      {
        Id = created.Id,
        UserId = created.UserId,
        SystemResourceId = created.SystemResourceId,
        CreatedAt = created.CreatedAt,
        UpdatedAt = created.UpdatedAt
      };
    }
  }
}
