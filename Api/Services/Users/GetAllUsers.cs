using Api.Dtos;
using Api.Helpers;
using Api.Helpers.Pagination;
using Api.Interfaces.Repositories;

namespace Api.Services.Users;

public class GetAllUsers
{
    private readonly IUserRepository _repository;

    public GetAllUsers(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<UserListDto>> ExecuteAsync(
        int page = 1,
        int pageSize = 10)
    {
        Guard.AgainstNonPositiveInt(page, nameof(page));
        Guard.AgainstNonPositiveInt(pageSize, nameof(pageSize));

        var pagedUsers = await _repository.GetAllAsync(page, pageSize);

        return new PagedResult<UserListDto>
        {
            TotalItems = pagedUsers.TotalItems,
            Page = pagedUsers.Page,
            PageSize = pagedUsers.PageSize,
            Data = pagedUsers.Data.Select(u => new UserListDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                CreatedAt = u.CreatedAt
            }).ToList()
        };
    }
}
