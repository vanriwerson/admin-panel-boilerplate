using Microsoft.EntityFrameworkCore;

namespace Api.Helpers.Pagination;

public class PagedResult<T>
{
    public int TotalItems { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }

    public int TotalPages =>
        PageSize > 0
            ? (int)Math.Ceiling((double)TotalItems / PageSize)
            : 0;

    public IReadOnlyList<T> Data { get; private set; } = [];

    private PagedResult() { }

    public static async Task<PagedResult<T>> CreateAsync(
        IQueryable<T> query,
        int page = 1,
        int pageSize = 10)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            Data = items
        };
    }
}
