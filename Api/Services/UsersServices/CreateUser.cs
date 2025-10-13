using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;

namespace Api.Services.UsersServices
{
    public class CreateUser
    {
        private readonly IGenericRepository<User> _userRepo;

        public CreateUser(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserReadDto> ExecuteAsync(UserCreateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = PasswordHashing.Generate(dto.Password),
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepo.CreateAsync(user);

            return new UserReadDto
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FullName = createdUser.FullName,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt
            };
        }
    }
}
