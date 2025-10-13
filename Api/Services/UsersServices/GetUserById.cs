using Api.Dtos;
using Api.Interfaces;
using Api.Models;

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
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
