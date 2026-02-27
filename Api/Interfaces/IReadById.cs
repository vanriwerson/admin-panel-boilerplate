namespace Api.Interfaces;

public interface IReadById<T>
{
    Task<T?> GetByIdAsync(int id);
}
