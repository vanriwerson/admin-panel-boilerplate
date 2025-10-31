using Api.Dtos;
using Api.Helpers;
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

    public async Task<PaginatedResult<SystemResourceReadDto>> ExecuteAsync(string searchKey, int page = 1, int pageSize = 10)
    {
      var query = _repo.Query().Where(r =>
          r.Active == true && (
          EF.Functions.ILike(r.Name, $"%{searchKey}%") ||
          EF.Functions.ILike(r.ExhibitionName, $"%{searchKey}%")
      ));

      var paginatedResources = await ApplyPagination.PaginateAsync(query, page, pageSize);

      var resourceDtos = paginatedResources.Data.Select(r => new SystemResourceReadDto
      {
        Id = r.Id,
        Name = r.Name,
        ExhibitionName = r.ExhibitionName,
        Active = r.Active,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
      }).ToList();

      return new PaginatedResult<SystemResourceReadDto>
      {
        Data = resourceDtos,
        TotalItems = paginatedResources.TotalItems,
        Page = paginatedResources.Page,
        PageSize = paginatedResources.PageSize
      };
    }
  }
}
