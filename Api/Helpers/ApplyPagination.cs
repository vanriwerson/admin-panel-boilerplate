using Microsoft.EntityFrameworkCore;

namespace Api.Helpers
{
  public class PaginatedResult<T>
  {
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
  }

  public static class ApplyPagination
  {
    public static async Task<PaginatedResult<T>> PaginateAsync<T>(
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

      return new PaginatedResult<T>
      {
        TotalItems = totalItems,
        Page = page,
        PageSize = pageSize,
        Data = items
      };
    }
  }
}
