namespace Api.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> SoftDeleteAsync(int id);

    Task<User?> GetByIdAsync(int id);
    Task<PagedResult<User>> GetAllAsync(int page, int pageSize);
    Task<IEnumerable<User>> GetForSelectAsync();
    Task<PagedResult<User>> SearchAsync(string term, int page, int pageSize);

    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}
