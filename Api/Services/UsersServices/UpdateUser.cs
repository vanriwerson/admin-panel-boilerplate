using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;

namespace Api.Services.UsersServices
{
    public class UpdateUser
    {
        private readonly IGenericRepository<User> _userRepo;

        public UpdateUser(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserReadDto?> ExecuteAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            user.Username = dto.Username ?? user.Username;
            user.Email = dto.Email ?? user.Email;
            user.FullName = dto.FullName ?? user.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = PasswordHashing.Generate(dto.Password);

            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepo.UpdateAsync(user);

            return new UserReadDto
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                FullName = updatedUser.FullName,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            };
        }
    }
}
