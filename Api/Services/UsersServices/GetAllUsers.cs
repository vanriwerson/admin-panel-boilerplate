using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;

namespace Api.Services.UsersServices
{
    public class GetAllUsers
    {
        private readonly IGenericRepository<User> _userRepo;

        public GetAllUsers(IGenericRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<PaginatedResult<UserReadDto>> ExecuteAsync(int page = 1, int pageSize = 10)
        {
            var query = _userRepo.Query();
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
