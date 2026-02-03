namespace Api.Interfaces.Repositories;

public interface ISystemLogRepository
{
    Task<SystemLog> CreateAsync(SystemLog log);

    Task<SystemLog?> GetByIdAsync(int id);
    Task<PagedResult<SystemLog>> GetPagedAsync(int page, int pageSize);
}
