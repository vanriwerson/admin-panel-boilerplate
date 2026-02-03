using Api.Dtos;
using Api.Interfaces;
using Api.Models;
using Api.Validations;
using Microsoft.EntityFrameworkCore;

namespace Api.Auditing;

public class AuditLogQueryService
{
    private readonly IGenericRepository<SystemLog> _logRepo;

    public AuditLogQueryService(IGenericRepository<SystemLog> logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task<PaginatedResult<SystemLogReadDto>> ExecuteAsync(
        int? userId = null,
        string? action = null,
        string? startDate = null,
        string? endDate = null,
        int page = 1,
        int pageSize = 10
    )
    {
        DateTime? startDateTime = null;
        DateTime? endDateTime = null;

        if (!string.IsNullOrEmpty(startDate)
            && DateTime.TryParse(startDate, out var parsedStart))
        {
            startDateTime = DateTime.SpecifyKind(
                parsedStart.Date,
                DateTimeKind.Utc
            );
        }

        if (!string.IsNullOrEmpty(endDate)
            && DateTime.TryParse(endDate, out var parsedEnd))
        {
            endDateTime = DateTime.SpecifyKind(
                parsedEnd.Date.AddDays(1),
                DateTimeKind.Utc
            );
        }

        ValidateDateRange.EnsureValidPeriod(startDateTime, endDateTime);

        var query = _logRepo.Query()
            .Include(sl => sl.User)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(sl => sl.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(sl =>
                EF.Functions.Like(
                    sl.Action.ToLower(),
                    $"%{action.ToLower()}%"
                )
            );

        if (startDateTime.HasValue)
            query = query.Where(sl => sl.CreatedAt >= startDateTime.Value);

        if (endDateTime.HasValue)
            query = query.Where(sl => sl.CreatedAt < endDateTime.Value);

        query = query.OrderByDescending(sl => sl.CreatedAt);

        var report = query.Select(sl => new SystemLogReadDto
        {
            Id = sl.Id,
            UserId = sl.UserId,
            Action = sl.Action,
            UsedPayload = sl.UsedPayload,
            CreatedAt = sl.CreatedAt,
            User = sl.User != null
                ? new UserLogReadDto
                {
                    Id = sl.User.Id,
                    Username = sl.User.Username,
                    Email = sl.User.Email,
                    FullName = sl.User.FullName
                }
                : null!
        });

        return await ApplyPagination.PaginateAsync(
            report,
            page,
            pageSize
        );
    }
}
