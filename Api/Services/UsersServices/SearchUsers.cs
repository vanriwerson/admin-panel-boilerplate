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
            var query = _userRepo.Query()
                .Include(u => u.AccessPermissions)
                .ThenInclude(ap => ap.SystemResource)
                .Where(u =>
                    u.Active == true &&
                    (
                    EF.Functions.ILike(u.Username, $"%{searchKey}%") ||
                    EF.Functions.ILike(u.Email, $"%{searchKey}%") ||
                    EF.Functions.ILike(u.FullName, $"%{searchKey}%")
                    ))
                .OrderBy(u => u.FullName)
                .Select(u => UserMapper.MapToUserReadDto(u));

            var paginatedUsers = await ApplyPagination.PaginateAsync(query, page, pageSize);

            return paginatedUsers;
        }
    }
}
