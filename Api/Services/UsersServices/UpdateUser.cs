using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
            if (!ValidateEntity.HasValidProperties<UserUpdateDto>(dto))
                throw new AppException("A requisição não possui os campos esperados.", (int)HttpStatusCode.BadRequest);

            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Email) || !string.IsNullOrWhiteSpace(dto.Username))
            {
                bool isDuplicate = await _userRepo.Query()
                    .AnyAsync(u =>
                        u.Id != id &&
                        ((dto.Email != null && u.Email == dto.Email) ||
                         (dto.Username != null && u.Username == dto.Username))
                    );

                if (isDuplicate)
                    throw new AppException("Email ou Username já cadastrado por outro usuário.", (int)HttpStatusCode.Conflict);
            }

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
