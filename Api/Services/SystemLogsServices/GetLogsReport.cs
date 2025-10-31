using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using Api.Validations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Services
{
  public class GetLogsReport
  {
    private readonly IGenericRepository<SystemLog> _logRepo;

    public GetLogsReport(IGenericRepository<SystemLog> logRepo)
    {
      _logRepo = logRepo;
    }

    public async Task<PaginatedResult<SystemLogReadDto>> ExecuteAsync(
        int? userId = null,
        string? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int page = 1,
        int pageSize = 10)
    {
      ValidateDateRange.EnsureValidPeriod(startDate, endDate);

      var query = _logRepo.Query()
          .Include(sl => sl.User)
          .AsQueryable();

      if (userId.HasValue)
        query = query.Where(sl => sl.UserId == userId.Value);

      if (!string.IsNullOrWhiteSpace(action))
        query = query.Where(sl =>
          EF.Functions.Like(sl.Action.ToLower(), $"%{action.ToLower()}%"));

      if (startDate.HasValue)
        query = query.Where(sl => sl.CreatedAt >= startDate.Value);

      if (endDate.HasValue)
      {
        var endOfDay = endDate.Value.Date.AddDays(1);
        query = query.Where(sl => sl.CreatedAt < endOfDay);
      }

      query = query.OrderByDescending(sl => sl.CreatedAt);

      var report = query.Select(sl => new SystemLogReadDto
      {
        Id = sl.Id,
        UserId = sl.UserId,
        Action = sl.Action,
        CreatedAt = sl.CreatedAt,
        User = sl.User != null
              ? new UserLogReadDto
              {
                Id = sl.User.Id,
                Username = sl.User.Username,
                Email = sl.User.Email,
                FullName = sl.User.FullName
              }
              : new UserLogReadDto()
      });

      var paginatedLogs = await ApplyPagination.PaginateAsync(report, page, pageSize);
      return paginatedLogs;
    }
  }
}
