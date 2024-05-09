using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace GenericServer.Controllers;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;
    public BaseRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<T?> CreateAsync(T entity)
    {
        var addedEntity = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return addedEntity.Entity;
    }

    public virtual async Task<IQueryable<T>?> ReadAllAsync()
        => await Task.Run(() => _dbSet.AsNoTracking());

    public virtual async Task<IQueryable<T>?> ReadAllWithIncludesAsync(params string[] includes)
    {
        return await Task.Run
            (
                () =>
                {
                    var allEnities = _dbSet.AsQueryable();
                    foreach (var include in includes)
                    {
                        allEnities = allEnities.Include(include);
                    }
                    return allEnities;
                }
            );
    }

    public virtual async Task<T?> ReadByIdAsync(object id) => await _dbSet.FindAsync(id);

    public virtual async Task<T?> ReadByIdWithIncludesAsync(object id, params string[] includes)
    {
        var entity = _dbSet.AsQueryable();
        foreach (var include in includes)
        {
            entity = entity.Include(include);
        }
        var entit = await _dbSet.FindAsync(id);
        return await entity.FirstOrDefaultAsync(entity => entity == entit);
    }

    public virtual async Task<T?> UpdateAsync(Guid id, T updatedEntity)
    {
        var existingEntity = await _dbSet.FindAsync(id);
        if (existingEntity is null) return null;

        var poperties = typeof(T).GetProperties();

        foreach (var property in poperties)
        {
            if (property.Name == "Id") continue;

            var newValue = property.GetValue(updatedEntity);

            if (newValue is null) continue;
            property.SetValue(existingEntity, newValue);
        }

        await _context.SaveChangesAsync();

        return updatedEntity;
    }

    public virtual async Task<T?> DeleteAsync(Guid id)
    {
        var entityToRemove = await _dbSet.FindAsync(id);
        if (entityToRemove != null)
        {
            _dbSet.Remove(entityToRemove);
            await _context.SaveChangesAsync();
        }
        return entityToRemove;
    }

    public virtual async Task<List<T>> FindAsync(string key, string? value)
    {
        var property = typeof(T).GetProperty(key);

        if (property == null)
            throw new ArgumentException($"'{typeof(T).Name}' does not implement a public get property named '{key}'.");

        if (value == null) return _dbSet.ToList();

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var containsMethod = typeof(string).GetMethod("Contains", [typeof(string)]);
        var valueExpression = Expression.Constant(value, typeof(string));
        var containsCall = Expression.Call(propertyAccess, containsMethod, valueExpression);
        var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

        return await _dbSet.Where(lambda).ToListAsync();
    }

}