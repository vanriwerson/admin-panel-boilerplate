using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Middlewares;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
            if (!ValidateEntity.HasValidProperties<UserCreateDto>(dto))
                throw new AppException("A requisição não possui os campos esperados.", (int)HttpStatusCode.BadRequest);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Password = PasswordHashing.Generate(dto.Password),
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (await _userRepo.Query().AnyAsync(u => u.Email == user.Email || u.Username == user.Username))
                throw new AppException("Email ou Username já cadastrado.", (int)HttpStatusCode.Conflict);

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
