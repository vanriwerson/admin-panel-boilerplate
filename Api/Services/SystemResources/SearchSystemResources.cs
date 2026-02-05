using Api.Dtos;
using Api.Helpers;
using Api.Helpers.Pagination;
using Api.Interfaces.Repositories;

namespace Api.Services.SystemResources;

public class SearchSystemResources
{
    private readonly ISystemResourceRepository _repository;

    public SearchSystemResources(ISystemResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SystemResourceReadDto>> ExecuteAsync(
      string term,
      int page = 1,
      int pageSize = 10)
    {
        Guard.AgainstNullOrEmpty(term, nameof(term));
        Guard.AgainstNonPositiveInt(page);
        Guard.AgainstNonPositiveInt(pageSize);

        var pagedResources = await _repository.SearchAsync(term, page, pageSize);

        var data = pagedResources.Data.Select(r => new SystemResourceReadDto
        {
            Id = r.Id,
            Name = r.Name,
            ExhibitionName = r.ExhibitionName
        }).ToList();

        return new PagedResult<SystemResourceReadDto>(
          pagedResources.TotalItems,
          pagedResources.Page,
          pagedResources.PageSize,
          data
        );
    }
}
