using Api.Dtos;
using Api.Helpers;
using Api.Interfaces;
using Api.Models;
using Microsoft.EntityFrameworkCore;

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
            var query = _userRepo.Query()
                .Include(u => u.AccessPermissions)
                .ThenInclude(ap => ap.SystemResource)
                .Where(u => u.Active)
                .OrderBy(u => u.FullName)
                .Select(u => UserMapper.MapToUserReadDto(u));

            var paginatedUsers = await ApplyPagination.PaginateAsync(query, page, pageSize);
            return paginatedUsers;
        }

        public async Task<IEnumerable<UserLogReadDto>> GetOptionsAsync()
        {
            var options = await _userRepo.Query()
                .AsNoTracking()
                .OrderBy(u => u.FullName)
                .Select(u => new UserLogReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName
                })
                .ToListAsync();

            return options;
        }
    }
}
