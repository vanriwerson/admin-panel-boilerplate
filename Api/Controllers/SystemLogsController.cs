using Api.Dtos;
using Api.Helpers;
using Api.Services.SystemLogsServices;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
  [Route("api/reports")]
  public class SystemLogsController : ControllerBase
  {
    private readonly GetLogsReport _getLogsReport;

    public SystemLogsController(GetLogsReport getLogsReport)
    {
      _getLogsReport = getLogsReport;
    }

    // Exemplos de chamadas:
    // GET /api/reports?userId=5&page=1&pageSize=20
    // GET /api/reports?action=DELETE
    // GET /api/reports?startDate=2025-01-01&endDate=2025-02-01
    [HttpGet]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
      if (startDate.HasValue)
        startDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);

      if (endDate.HasValue)
        endDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);

      var logs = await _getLogsReport.ExecuteAsync(userId, action, startDate, endDate, page, pageSize);

      return Ok(logs);
    }
  }
}
