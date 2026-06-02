using Api.Dtos;
using Api.Models;

namespace Api.Interfaces.Auditing.Services;

public interface ICreateSystemLog
{
    Task ExecuteAsync(
        string action,
        int? userId = null,
        string? generatedBy = null,
        SystemLogDataDto? data = null
    );
}