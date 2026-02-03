using Api.Dtos;
using Api.Interfaces.Repositories;
using Api.Validations;

namespace Api.Auditing;

public class SystemLogQueryService
{
    private readonly ISystemLogRepository _logRepository;

    public SystemLogQueryService(ISystemLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<PagedResult<SystemLogReadDto>> ExecuteAsync(
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

        if (!string.IsNullOrWhiteSpace(startDate)
            && DateTime.TryParse(startDate, out var parsedStart))
        {
            startDateTime = DateTime.SpecifyKind(parsedStart.Date, DateTimeKind.Utc);
        }

        if (!string.IsNullOrWhiteSpace(endDate)
            && DateTime.TryParse(endDate, out var parsedEnd))
        {
            endDateTime = DateTime.SpecifyKind(parsedEnd.Date.AddDays(1), DateTimeKind.Utc);
        }

        ValidateDateRange.EnsureValidPeriod(startDateTime, endDateTime);

        return await _logRepository.GetPagedAsync(
            userId,
            action,
            startDateTime,
            endDateTime,
            page,
            pageSize
        );
    }
}
