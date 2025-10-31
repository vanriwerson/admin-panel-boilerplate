using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.SystemResourcesServices
{
  public class GetAllSystemResources
  {
    private readonly IGenericRepository<SystemResource> _repo;

    public GetAllSystemResources(IGenericRepository<SystemResource> repo)
    {
      _repo = repo;
    }

    public async Task<PaginatedResult<SystemResourceReadDto>> ExecuteAsync(int page = 1, int pageSize = 10)
    {
      var query = _repo.Query()
          .AsNoTracking()
          .Where(r => r.Active)
          .OrderBy(r => r.Id)
          .Select(r => new SystemResourceReadDto
          {
            Id = r.Id,
            Name = r.Name,
            ExhibitionName = r.ExhibitionName,
            Active = r.Active,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
          });

      return await ApplyPagination.PaginateAsync(query, page, pageSize);
    }

    public async Task<IEnumerable<SystemResourceOptionDto>> GetOptionsAsync()
    {
      var options = await _repo.Query()
          .AsNoTracking()
          .Where(r => r.Active)
          .Select(r => new SystemResourceOptionDto
          {
            Id = r.Id,
            Name = r.Name,
            ExhibitionName = r.ExhibitionName
          })
          .ToListAsync();

      return options;
    }
  }
}
