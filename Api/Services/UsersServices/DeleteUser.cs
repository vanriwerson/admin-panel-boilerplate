using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using Api.Services;

namespace Api.Services.UsersServices
{
    public class DeleteUser
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly CreateSystemLog _createSystemLog;

        public DeleteUser(IGenericRepository<User> userRepo, CreateSystemLog createSystemLog)
        {
            _userRepo = userRepo;
            _createSystemLog = createSystemLog;
        }

        public async Task<bool> ExecuteAsync(int id)
        {
            var deleted = await _userRepo.DeleteAsync(id);

            if (deleted)
            {
                await _createSystemLog.ExecuteAsync(
                    action: LogActionDescribe.Delete("User", id)
                );
            }

            return deleted;
        }
    }
}
