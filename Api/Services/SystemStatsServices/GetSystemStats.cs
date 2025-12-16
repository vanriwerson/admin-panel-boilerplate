using Api.Dtos;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Api.Services.SystemStatsServices
{
  public class GetSystemStats
  {
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<SystemResource> _systemResourceRepo;
    private readonly IGenericRepository<SystemLog> _systemLogRepo;

    public GetSystemStats(
        IGenericRepository<User> userRepo,
        IGenericRepository<SystemResource> systemResourceRepo,
        IGenericRepository<SystemLog> systemLogRepo)
    {
      _userRepo = userRepo;
      _systemResourceRepo = systemResourceRepo;
      _systemLogRepo = systemLogRepo;
    }

    public async Task<GeneralSystemStatsDto> ExecuteAsync()
    {
      var usersCount = await _userRepo.Query().Where(u => u.Active).CountAsync();
      var systemResourcesCount = await _systemResourceRepo.Query().Where(r => r.Active).CountAsync();

      var currentMonth = DateTime.UtcNow.Month;
      var currentYear = DateTime.UtcNow.Year;

      var monthlyReportsCount = await _systemLogRepo.Query()
          .Where(l => l.CreatedAt.Month == currentMonth && l.CreatedAt.Year == currentYear)
          .CountAsync();

      var culture = new CultureInfo("pt-BR");
      var monthName = culture.DateTimeFormat.GetMonthName(currentMonth);
      var capitalizedMonth = char.ToUpper(monthName[0]) + monthName.Substring(1);
      var monthlyReportsCountReference = $"{capitalizedMonth}/{currentYear}";

      return new GeneralSystemStatsDto
      {
        UsersCount = usersCount,
        SystemResourcesCount = systemResourcesCount,
        MonthlyReportsCount = monthlyReportsCount,
        MonthlyReportsCountReference = monthlyReportsCountReference
      };
    }
  }
}