using Api.Helpers;
using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
  public class CreateSystemLog
  {
    private readonly IGenericRepository<SystemLog> _repository;
    private readonly CurrentAuthUser _currentAuthUser;

    public CreateSystemLog(IGenericRepository<SystemLog> repository, CurrentAuthUser currentAuthUser)
    {
      _repository = repository;
      _currentAuthUser = currentAuthUser;
    }

    public async Task ExecuteAsync(string action, int? userId = null, string? data = null)
    {
      var logUserId = userId ?? _currentAuthUser.GetId();

      var log = new SystemLog
      {
        UserId = logUserId,
        Action = action,
        UsedPayload = data,
        CreatedAt = DateTime.UtcNow
      };

      await _repository.CreateAsync(log);
    }
  }
}
