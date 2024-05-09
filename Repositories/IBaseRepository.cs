namespace GenericServer.Controllers;

public interface IBaseRepository<T> where T : class
{
    Task<T?> CreateAsync(T entity);
    Task<T?> DeleteAsync(Guid id);
    Task<List<T>> FindAsync(string key, string? value);
    Task<IQueryable<T>?> ReadAllAsync();
    Task<IQueryable<T>?> ReadAllWithIncludesAsync(params string[] includes);
    Task<T?> ReadByIdAsync(object id);
    Task<T?> ReadByIdWithIncludesAsync(object id, params string[] includes);
    Task<T?> UpdateAsync(Guid id, T entity);
}
