using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace GenericServer.Controllers;
/// <summary>
/// Base controller providing CRUD operations for entities.
/// </summary>
/// <typeparam name="T">The type of entity managed by the controller.</typeparam>
/// <typeparam name="TDTO">The DTO (Data Transfer Object) type for the entity.</typeparam>

[Route("api/[controller]")]
[ApiController]
public class BaseController<T,TDTO>(IBaseRepository<T> baseRepository) : ControllerBase where T : class
{
    private readonly IBaseRepository<T> _baseRepository = baseRepository;

    // GET: api/controller/GetAll
    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            IQueryable<T>? entities = await _baseRepository.ReadAllAsync();

            if (entities is null)
                return NoContent();


            return Ok(entities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/controller/GetAllWithIncludes
    /// <summary>
    /// Retrieves all entities with specified related entities included.
    /// </summary>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>A list of all entities with specified related entities included.</returns>
    [HttpGet("GetAllWithIncludes")]
    public async Task<IActionResult> GetAllWithIncludes([FromQuery]params string[] includes)
    {
        try
        {
            IQueryable<T>? entities = await _baseRepository.ReadAllWithIncludesAsync(includes);

            if (entities is null)
                return NoContent();

            return Ok(entities.ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/controller/{id}
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The entity with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            T? entity = await _baseRepository.ReadByIdAsync(id);

            if (entity is null)
                return NoContent();

            return Ok(entity);
        }
        catch (Exception ex)
        {

            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/controller/WithIncludes/{id}
    /// <summary>
    /// Retrieves an entity by its ID with specified related entities included.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="includes">The related entities to include.</param>
    /// <returns>The entity with the specified ID and related entities included.</returns>
    [HttpGet("WithIncludes/{id}")]
    public async Task<IActionResult> GetByIdWithIncludesAsync(Guid id,[FromQuery] params string[] includes)
    {
        try
        {
            T? entity = await _baseRepository.ReadByIdWithIncludesAsync(id, includes);

            if (entity is null)
                return NoContent();

            return Ok(entity);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // POST: api/controller
    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The created entity.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TDTO entity)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string json = JsonSerializer.Serialize(entity);

            T castedEntity = JsonSerializer.Deserialize<T>(json);

            T? createdEntity = await _baseRepository.CreateAsync(castedEntity);

            if (createdEntity == null)
                return StatusCode(500, "Error creating entity");

            return CreatedAtRoute(new { id = createdEntity.GetType().GUID }, createdEntity);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }


    // PUT: api/controller/{id}
    /// <summary>
    /// Updates an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to update.</param>
    /// <param name="entity">The updated entity.</param>
    /// <returns>The updated entity.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] TDTO entity)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string json = JsonSerializer.Serialize(entity);
            T castedEntity = JsonSerializer.Deserialize<T>(json);

            if (id.Equals(Guid.Empty))
                return BadRequest("Invalid ID");


            T? existingEntity = await _baseRepository.ReadByIdAsync(id);

            if (existingEntity is null)
                return NotFound("The Entity was not found");
            
            T? updatedEntity = await _baseRepository.UpdateAsync(id, castedEntity);

            if (updatedEntity is null)
                return StatusCode(500, "Error creating entity");

            return Ok(updatedEntity);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return StatusCode(500, ex.Message);
        }

    }

    // DELETE: api/controller/{id}
    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>The deleted entity.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {

            if (id.Equals(Guid.Empty))
                return BadRequest("Invalid ID");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            T? existingEntity = await _baseRepository.ReadByIdAsync(id);
            if (existingEntity is null)
                return NotFound("The Entity was not found");

            T? deletedEntity = await _baseRepository.DeleteAsync(id);
            if (deletedEntity is null)
                return StatusCode(500, "Error deleting entity");

            return Ok(deletedEntity);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    // GET: api/controller/Find
    /// <summary>
    /// Finds entities by a specified property and value.
    /// </summary>
    /// <param name="key">The property name to search by.</param>
    /// <param name="value">The value to search for.</param>
    /// <returns>A list of entities that match the search criteria.</returns>
    [HttpGet("Find")]
    public async Task<IActionResult> Find([FromQuery] string key, string? value)
    {
        var result = await _baseRepository.FindAsync(key, value);

        return Ok(result);
    }
}
