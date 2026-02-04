using Api.Dtos;
using Api.Helpers.Pagination;
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

        public async Task<PagedResult<SystemResourceReadDto>> ExecuteAsync(int page = 1, int pageSize = 10)
        {
            var query = _repo.Query()
                .AsNoTracking()
                .Where(r => r.Active)
                .OrderBy(r => r.Id)
                .Select(r => new SystemResourceReadDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    ExhibitionName = r.ExhibitionName
                });

            return await PagedResult<SystemResourceReadDto>.CreateAsync(query, page, pageSize);
        }

        public async Task<IEnumerable<SystemResourceSelectDto>> GetOptionsAsync()
        {
            var options = await _repo.Query()
                .AsNoTracking()
                .Where(r => r.Active)
                .Select(r => new SystemResourceSelectDto
                {
                    Id = r.Id,
                    ExhibitionName = r.ExhibitionName
                })
                .ToListAsync();

            return options;
        }
    }
}
