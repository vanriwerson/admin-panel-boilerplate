using Api.Interfaces;
using Api.Models;
using Api.Dtos;
using Api.Middlewares;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Api.Services.AccessPermissionsServices
{
  public class CreateAccessPermission
  {
    private readonly IGenericRepository<AccessPermission> _permissionRepo;
    private readonly IGenericRepository<SystemResource> _resourceRepo;

    public CreateAccessPermission(
        IGenericRepository<AccessPermission> permissionRepo,
        IGenericRepository<SystemResource> resourceRepo)
    {
      _permissionRepo = permissionRepo;
      _resourceRepo = resourceRepo;
    }

    public async Task ExecuteAsync(AccessPermissionCreateDto dto)
    {
      if (dto.UserId <= 0 || dto.SystemResourceId <= 0)
        throw new AppException("Usuário ou recurso inválido.", (int)HttpStatusCode.BadRequest);

      var resourceExists = await _resourceRepo.Query()
          .AnyAsync(r => r.Id == dto.SystemResourceId);

      if (!resourceExists)
        throw new AppException("Recurso não cadastrado.", (int)HttpStatusCode.NotFound);

      var permission = new AccessPermission
      {
        UserId = dto.UserId,
        SystemResourceId = dto.SystemResourceId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      await _permissionRepo.CreateAsync(permission);
    }
  }
}
