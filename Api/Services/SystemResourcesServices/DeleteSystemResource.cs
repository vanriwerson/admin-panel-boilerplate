using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Api.Services;
using System.Net;

namespace Api.Services.SystemResourcesServices
{
  public class DeleteSystemResource
  {
    private readonly IGenericRepository<SystemResource> _repo;
    private readonly CreateSystemLog _createSystemLog;

    public DeleteSystemResource(IGenericRepository<SystemResource> repo, CreateSystemLog createSystemLog)
    {
      _repo = repo;
      _createSystemLog = createSystemLog;
    }

    public async Task<bool> ExecuteAsync(int id)
    {
      if (id <= 0)
        throw new AppException("ID inválido.", (int)HttpStatusCode.BadRequest);

      var success = await _repo.DeleteAsync(id);

      if (!success)
        throw new AppException("Recurso não encontrado.", (int)HttpStatusCode.NotFound);

      await _createSystemLog.ExecuteAsync(
          action: LogActionDescribe.Delete("SystemResource", id)
      );

      return true;
    }
  }
}
