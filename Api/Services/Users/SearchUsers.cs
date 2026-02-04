using Api.Dtos;
using Api.Helpers;
using Api.Helpers.Pagination;
using Api.Interfaces.Repositories;

namespace Api.Services.Users;

public class SearchUsers
{
    private readonly IUserRepository _repository;

    public SearchUsers(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<UserListDto>> ExecuteAsync(
        string term,
        int page = 1,
        int pageSize = 10)
    {
        Guard.AgainstNullOrEmpty(term, nameof(term));
        Guard.AgainstNonPositiveInt(page);
        Guard.AgainstNonPositiveInt(pageSize);

        var pagedUsers = await _repository.SearchAsync(term, page, pageSize);

        var data = pagedUsers.Data.Select(u => new UserListDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            CreatedAt = u.CreatedAt
        }).ToList();

        return new PagedResult<UserListDto>(
            pagedUsers.TotalItems,
            pagedUsers.Page,
            pagedUsers.PageSize,
            data);
    }
}
