using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.UsersServices
{
    public class GetUserById
    {
        private readonly IGenericRepository<User> _userRepo;

        public GetUserById(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserReadDto?> ExecuteAsync(int id)
        {
            var user = await _userRepo.Query()
                .Include(u => u.AccessPermissions)
                .ThenInclude(ap => ap.SystemResource)
                .Where(u => u.Active)
                .FirstOrDefaultAsync(u => u.Id == id);

            return user == null ? null : UserMapper.MapToUserReadDto(user);
        }
    }
}
