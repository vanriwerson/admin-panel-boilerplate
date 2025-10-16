using Api.Interfaces;
using Api.Models;
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

    public async Task<AccessPermission> ExecuteAsync(int userId, int systemResourceId)
    {
      if (userId <= 0 || systemResourceId <= 0)
        throw new AppException("Usuário ou recurso inválido.", (int)HttpStatusCode.BadRequest);

      var permission = new AccessPermission
      {
        UserId = userId,
        SystemResourceId = systemResourceId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      return await _repo.CreateAsync(permission);
    }
  }
}
