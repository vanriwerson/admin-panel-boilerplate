using Api.Interfaces;
using Api.Models;
using Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.AccessPermissionsServices
{
  public class ListAccessPermissions
  {
    private readonly IGenericRepository<AccessPermission> _repo;

    public ListAccessPermissions(IGenericRepository<AccessPermission> repo)
    {
      _repo = repo;
    }

    public async Task<List<AccessPermissionReadDto>> ExecuteAsync(int userId)
    {
      var permissions = await _repo.Query()
          .Where(ap => ap.UserId == userId)
          .ToListAsync();

      return permissions.Select(ap => new AccessPermissionReadDto
      {
        Id = ap.Id,
        UserId = ap.UserId,
        SystemResourceId = ap.SystemResourceId,
        CreatedAt = ap.CreatedAt,
        UpdatedAt = ap.UpdatedAt
      }).ToList();
    }
  }
}
