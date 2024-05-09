using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json;

namespace GenericServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController<T,TDTO>(IBaseRepository<T> baseRepository) : ControllerBase where T : class
{
    private readonly IBaseRepository<T> _baseRepository = baseRepository;

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

    // DELETE api/<WalletsController>/5
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

    [HttpGet("Find")]
    public async Task<IActionResult> Find([FromQuery] string key, string? value)
    {
        var result = await _baseRepository.FindAsync(key, value);

        return Ok(result);
    }
}
