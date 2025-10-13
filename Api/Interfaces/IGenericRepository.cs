using System.Linq.Expressions;

namespace Api.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
