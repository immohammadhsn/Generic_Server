using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericServer.Controllers
{
    /// <summary>
    /// Generic repository for CRUD operations.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        /// <summary>
        /// The database context.
        /// </summary>
        protected readonly DbContext _context;

        /// <summary>
        /// The database set for the entity.
        /// </summary>
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public BaseRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Asynchronously creates a new entity in the database.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>The created entity.</returns>
        public virtual async Task<T?> CreateAsync(T entity)
        {
            var addedEntity = await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return addedEntity.Entity;
        }

        /// <summary>
        /// Asynchronously reads all entities from the database.
        /// </summary>
        /// <returns>All entities in the database.</returns>
        public virtual async Task<IQueryable<T>?> ReadAllAsync()
            => await Task.Run(() => _dbSet.AsNoTracking());

        /// <summary>
        /// Asynchronously reads all entities from the database with the specified related entities included.
        /// </summary>
        /// <param name="includes">The related entities to include.</param>
        /// <returns>All entities with the specified related entities included.</returns>
        public virtual async Task<IQueryable<T>?> ReadAllWithIncludesAsync(params string[] includes)
        {
            return await Task.Run
            (
                () =>
                {
                    var allEntities = _dbSet.AsQueryable();
                    foreach (var include in includes)
                    {
                        allEntities = allEntities.Include(include);
                    }
                    return allEntities;
                }
            );
        }

        /// <summary>
        /// Asynchronously reads an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to read.</param>
        /// <returns>The entity with the specified ID.</returns>
        public virtual async Task<T?> ReadByIdAsync(object id) => await _dbSet.FindAsync(id);

        /// <summary>
        /// Asynchronously reads an entity by its ID with the specified related entities included.
        /// </summary>
        /// <param name="id">The ID of the entity to read.</param>
        /// <param name="includes">The related entities to include.</param>
        /// <returns>The entity with the specified ID and included related entities.</returns>
        public virtual async Task<T?> ReadByIdWithIncludesAsync(object id, params string[] includes)
        {
            var entity = _dbSet.AsQueryable();
            foreach (var include in includes)
            {
                entity = entity.Include(include);
            }
            var entit = await _dbSet.FindAsync(id);
            return await entity.FirstOrDefaultAsync(e => e == entit);
        }

        /// <summary>
        /// Asynchronously updates an entity in the database.
        /// </summary>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="updatedEntity">The updated entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual async Task<T?> UpdateAsync(Guid id, T updatedEntity)
        {
            var existingEntity = await _dbSet.FindAsync(id);
            if (existingEntity is null) return null;

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "Id") continue;

                var newValue = property.GetValue(updatedEntity);

                if (newValue is null) continue;
                property.SetValue(existingEntity, newValue);
            }

            await _context.SaveChangesAsync();

            return updatedEntity;
        }

        /// <summary>
        /// Asynchronously deletes an entity from the database.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        /// <returns>The deleted entity.</returns>
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

        /// <summary>
        /// Asynchronously finds entities by a specified property and value.
        /// </summary>
        /// <param name="key">The property name to search by.</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>A list of entities that match the search criteria.</returns>
        public virtual async Task<List<T>> FindAsync(string key, string? value)
        {
            var property = typeof(T).GetProperty(key);

            if (property == null)
                throw new ArgumentException($"'{typeof(T).Name}' does not implement a public get property named '{key}'.");

            if (value == null) return _dbSet.ToList();

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, property);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var valueExpression = Expression.Constant(value, typeof(string));
            var containsCall = Expression.Call(propertyAccess, containsMethod, valueExpression);
            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

            return await _dbSet.Where(lambda).ToListAsync();
        }
    }
}
