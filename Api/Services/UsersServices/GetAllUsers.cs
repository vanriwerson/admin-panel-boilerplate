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
                .ThenInclude(ap => ap.SystemResource);

            var paginatedUsers = await ApplyPagination.PaginateAsync(query, page, pageSize);

            var userDtos = paginatedUsers.Data.Select(user => new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Permissions = user.AccessPermissions?
                    .Select(ap => new SystemResourceOptionDto
                    {
                        Id = ap.SystemResource.Id,
                        Name = ap.SystemResource.Name,
                        ExhibitionName = ap.SystemResource.ExhibitionName
                    })
                    .ToList() ?? new List<SystemResourceOptionDto>()
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
