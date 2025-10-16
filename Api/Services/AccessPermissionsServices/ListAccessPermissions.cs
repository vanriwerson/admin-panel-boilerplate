using Api.Interfaces;
using Api.Models;
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

    public async Task<List<AccessPermission>> ExecuteAsync(int userId)
    {
      return await _repo.Query()
                        .Where(ap => ap.UserId == userId)
                        .ToListAsync();
    }
  }
}
