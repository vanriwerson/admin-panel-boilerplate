namespace Api.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> SoftDeleteAsync(int id);

    Task<User?> GetByIdAsync(int id);
    Task<PagedResult<User>> GetPagedAsync(int page, int pageSize);

    Task<IEnumerable<User>> SearchAsync(string term);

    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}
