namespace GenericServer.Controllers;

/// <summary>
/// Interface for a generic repository providing basic CRUD operations.
/// </summary>
/// <typeparam name="T">The type of entity managed by the repository.</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Asynchronously creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    Task<T?> CreateAsync(T entity);

    /// <summary>
    /// Asynchronously deletes an entity from the repository by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>The deleted entity, if found; otherwise, null.</returns>
    Task<T?> DeleteAsync(Guid id);

    /// <summary>
    /// Asynchronously finds entities in the repository based on a specified property and value.
    /// </summary>
    /// <param name="key">The name of the property to search by.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>A list of entities that match the search criteria.</returns>
    Task<List<T>> FindAsync(string key, string? value);

    /// <summary>
    /// Asynchronously retrieves all entities from the repository.
    /// </summary>
    /// <returns>An IQueryable representing all entities.</returns>
    Task<IQueryable<T>?> ReadAllAsync();

    /// <summary>
    /// Asynchronously retrieves all entities from the repository with specified related entities included.
    /// </summary>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>An IQueryable representing all entities with specified related entities included.</returns>
    Task<IQueryable<T>?> ReadAllWithIncludesAsync(params string[] includes);

    /// <summary>
    /// Asynchronously retrieves an entity from the repository by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The entity with the specified ID, if found; otherwise, null.</returns>
    Task<T?> ReadByIdAsync(object id);

    /// <summary>
    /// Asynchronously retrieves an entity from the repository by its ID with specified related entities included.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>The entity with the specified ID and related entities included, if found; otherwise, null.</returns>
    Task<T?> ReadByIdWithIncludesAsync(object id, params string[] includes);

    /// <summary>
    /// Asynchronously updates an entity in the repository.
    /// </summary>
    /// <param name="id">The ID of the entity to update.</param>
    /// <param name="entity">The updated entity.</param>
    /// <returns>The updated entity, if found; otherwise, null.</returns>
    Task<T?> UpdateAsync(Guid id, T entity);
}
