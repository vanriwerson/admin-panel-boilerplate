namespace Api.Interfaces.Repositories;

public interface ISystemResourceRepository
{
    Task<SystemResource> CreateAsync(SystemResource resource);
    Task<SystemResource> UpdateAsync(SystemResource resource);
    Task<bool> SoftDeleteAsync(int id);

    Task<SystemResource?> GetByIdAsync(int id);
    Task<PagedResult<SystemResource>> GetPagedAsync(int page, int pageSize);

    Task<IEnumerable<SystemResource>> SearchAsync(string term);

    Task<bool> ExistsByNameAsync(string name);
}
