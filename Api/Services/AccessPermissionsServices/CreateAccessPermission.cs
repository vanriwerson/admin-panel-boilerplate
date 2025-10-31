using Api.Interfaces;
using Api.Models;
using Api.Dtos;
using Api.Validations;
using System.Net;
using System.Threading.Tasks;

namespace Api.Services
{
  public class CreateAccessPermission
  {
    private readonly IGenericRepository<AccessPermission> _permissionRepo;
    private readonly IGenericRepository<SystemResource> _resourceRepo;
    private readonly IGenericRepository<User> _userRepo;

    public CreateAccessPermission(
        IGenericRepository<AccessPermission> permissionRepo,
        IGenericRepository<SystemResource> resourceRepo,
        IGenericRepository<User> userRepo)
    {
      _permissionRepo = permissionRepo;
      _resourceRepo = resourceRepo;
      _userRepo = userRepo;
    }

    public async Task ExecuteAsync(AccessPermissionCreateDto dto)
    {
      await ValidateEntity.EnsureEntityExistsAsync(_userRepo, dto.UserId, "Usuário");
      await ValidateEntity.EnsureEntityExistsAsync(_resourceRepo, dto.SystemResourceId, "Recurso do sistema");

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
