using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.UsersServices
{
    public class SearchUsers
    {
        private readonly IGenericRepository<User> _userRepo;

        public SearchUsers(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<PaginatedResult<UserReadDto>> ExecuteAsync(string searchKey, int page = 1, int pageSize = 10)
        {
            var query = _userRepo.Query().Where(u =>
                EF.Functions.ILike(u.FullName, $"%{searchKey}%") ||
                EF.Functions.ILike(u.Username, $"%{searchKey}%") ||
                EF.Functions.ILike(u.Email, $"%{searchKey}%")
            );

            var paginatedUsers = await ApplyPagination.PaginateAsync(query, page, pageSize);

            var userDtos = paginatedUsers.Data.Select(user => new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();

            return new PaginatedResult<UserReadDto>
            {
                Data = userDtos,
                TotalItems = paginatedUsers.TotalItems,
                Page = paginatedUsers.Page,
                PageSize = paginatedUsers.PageSize
            };
        }
    }
}
