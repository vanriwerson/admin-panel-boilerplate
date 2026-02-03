namespace Api.Interfaces;

public interface IPagedRead<T>
{
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
}
