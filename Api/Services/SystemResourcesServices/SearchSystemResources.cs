using Api.Dtos;
using Api.Helpers.Pagination;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.SystemResourcesServices
{
  public class SearchSystemResources
  {
    private readonly IGenericRepository<SystemResource> _repo;

    public SearchSystemResources(IGenericRepository<SystemResource> repo)
    {
      _repo = repo;
    }

    public async Task<PagedResult<SystemResourceReadDto>> ExecuteAsync(string searchKey, int page = 1, int pageSize = 10)
    {
      var query = _repo.Query()
          .Where(r =>
              r.Active == true && (
              EF.Functions.ILike(r.Name, $"%{searchKey}%") ||
              EF.Functions.ILike(r.ExhibitionName, $"%{searchKey}%")
          ))
          .Select(r => new SystemResourceReadDto
          {
            Id = r.Id,
            Name = r.Name,
            ExhibitionName = r.ExhibitionName
          });

      return await PagedResult<SystemResourceReadDto>.CreateAsync(query, page, pageSize);
    }
  }
}
