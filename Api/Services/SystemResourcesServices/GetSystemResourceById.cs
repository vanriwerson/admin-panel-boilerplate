using Api.Dtos;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using System.Net;

namespace Api.Services.SystemResourcesServices
{
  public class GetSystemResourceById
  {
    private readonly IGenericRepository<SystemResource> _repo;

    public GetSystemResourceById(IGenericRepository<SystemResource> repo)
    {
      _repo = repo;
    }

    public async Task<SystemResourceReadDto> ExecuteAsync(int id)
    {
      if (id <= 0)
        throw new AppException("ID inválido.", (int)HttpStatusCode.BadRequest);

      var entity = await _repo.GetByIdAsync(id);

      if (entity == null)
        throw new AppException("Recurso não encontrado.", (int)HttpStatusCode.NotFound);

      return new SystemResourceReadDto
      {
        Id = entity.Id,
        Name = entity.Name,
        ExhibitionName = entity.ExhibitionName,
        Active = entity.Active,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
      };
    }
  }
}
