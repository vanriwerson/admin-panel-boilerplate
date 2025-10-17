using Api.Interfaces;
using Api.Models;
using Api.Dtos;

namespace Api.Services.AccessPermissionsServices
{
  public class UpdateAccessPermissions
  {
    private readonly IGenericRepository<AccessPermission> _repo;

    public UpdateAccessPermissions(IGenericRepository<AccessPermission> repo)
    {
      _repo = repo;
    }

    public async Task<List<AccessPermissionReadDto>> ExecuteAsync(int userId, List<int> systemResourceIds)
    {
      var existing = await _repo.SearchAsync(ap => ap.UserId == userId);

      foreach (var ap in existing)
        await _repo.DeleteAsync(ap.Id);

      var createdList = new List<AccessPermissionReadDto>();

      foreach (var resourceId in systemResourceIds)
      {
        var newPermission = new AccessPermission
        {
          UserId = userId,
          SystemResourceId = resourceId,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        };

        var created = await _repo.CreateAsync(newPermission);

        createdList.Add(new AccessPermissionReadDto
        {
          Id = created.Id,
          UserId = created.UserId,
          SystemResourceId = created.SystemResourceId,
          CreatedAt = created.CreatedAt,
          UpdatedAt = created.UpdatedAt
        });
      }

      return createdList;
    }
  }
}
