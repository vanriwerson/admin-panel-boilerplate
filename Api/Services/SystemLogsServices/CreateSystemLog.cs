using Api.Helpers;
using Api.Interfaces;
using Api.Models;

namespace Api.Services.SystemLogsServices
{
  public class CreateSystemLog
  {
    private readonly IGenericRepository<SystemLog> _repository;

    public CreateSystemLog(IGenericRepository<SystemLog> repository)
    {
      _repository = repository;
    }

    public async Task ExecuteAsync(int userId, string action)
    {
      var log = new SystemLog
      {
        UserId = userId,
        Action = action,
        CreatedAt = DateTime.UtcNow
      };

      await _repository.CreateAsync(log);
      Console.WriteLine($"[AUDIT LOG] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | user_id: {userId} | action: {action}");
    }
  }
}
