using Api.Dtos;
using Api.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Api.Services.SystemStatsServices;

public class GetSystemStats
{
    private readonly IUserRepository _userRepository;
    private readonly ISystemResourceRepository _systemResourceRepository;
    private readonly ISystemLogRepository _systemLogRepository;

    public GetSystemStats(
        IUserRepository userRepository,
        ISystemResourceRepository systemResourceRepository,
        ISystemLogRepository systemLogRepository)
    {
        _userRepository = userRepository;
        _systemResourceRepository = systemResourceRepository;
        _systemLogRepository = systemLogRepository;
    }

    public async Task<GeneralSystemStatsDto> ExecuteAsync()
    {
        var usersCount = await _userRepository
            .Query()
            .CountAsync();

        var systemResourcesCount = await _systemResourceRepository
            .Query()
            .CountAsync();

        var now = DateTime.UtcNow;

        var startOfMonth = new DateTime(
            now.Year,
            now.Month,
            1,
            0, 0, 0,
            DateTimeKind.Utc);

        var startOfNextMonth = startOfMonth.AddMonths(1);

        var monthlyReportsCount = await _systemLogRepository
            .Query()
            .Where(l =>
                l.CreatedAt >= startOfMonth &&
                l.CreatedAt < startOfNextMonth)
            .CountAsync();

        var culture = new CultureInfo("pt-BR");
        var monthName = culture.DateTimeFormat.GetMonthName(now.Month);
        var capitalizedMonth =
            char.ToUpper(monthName[0]) + monthName.Substring(1);

        return new GeneralSystemStatsDto
        {
            UsersCount = usersCount,
            SystemResourcesCount = systemResourcesCount,
            MonthlyReportsCount = monthlyReportsCount,
            MonthlyReportsCountReference =
                $"{capitalizedMonth}/{now.Year}"
        };
    }
}
