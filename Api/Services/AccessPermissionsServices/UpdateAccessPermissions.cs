using Api.Interfaces;
using Api.Models;

namespace Api.Services.AccessPermissionsServices
{
  public class UpdateAccessPermissions
  {
    private readonly IGenericRepository<AccessPermission> _repo;

    public UpdateAccessPermissions(IGenericRepository<AccessPermission> repo)
    {
      _repo = repo;
    }

    // Faz hard delete das permissões antigas e cria novas
    public async Task ExecuteAsync(int userId, List<int> systemResourceIds)
    {
      var existing = await _repo.SearchAsync(ap => ap.UserId == userId);

      foreach (var ap in existing)
      {
        await _repo.DeleteAsync(ap.Id);
      }

      foreach (var resourceId in systemResourceIds)
      {
        await _repo.CreateAsync(new AccessPermission
        {
          UserId = userId,
          SystemResourceId = resourceId,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        });
      }
    }
  }
}
