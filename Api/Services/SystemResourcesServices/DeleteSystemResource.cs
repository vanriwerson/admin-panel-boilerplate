using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using System.Net;

namespace Api.Services.SystemResourcesServices
{
  public class DeleteSystemResource
  {
    private readonly IGenericRepository<SystemResource> _repo;

    public DeleteSystemResource(IGenericRepository<SystemResource> repo)
    {
      _repo = repo;
    }

    public async Task<bool> ExecuteAsync(int id)
    {
      if (id <= 0)
        throw new AppException("ID inválido.", (int)HttpStatusCode.BadRequest);

      var success = await _repo.DeleteAsync(id);

      if (!success)
        throw new AppException("Recurso não encontrado.", (int)HttpStatusCode.NotFound);

      return true;
    }
  }
}
