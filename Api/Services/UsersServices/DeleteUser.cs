using Api.Interfaces;
using Api.Models;

namespace Api.Services.UsersServices
{
    public class DeleteUser
    {
        private readonly IGenericRepository<User> _userRepo;

        public DeleteUser(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<bool> ExecuteAsync(int id)
        {
            return await _userRepo.DeleteAsync(id);
        }
    }
}
