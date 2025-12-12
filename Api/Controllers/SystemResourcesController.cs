using Api.Dtos;
using Api.Helpers;
using Api.Services.SystemResourcesServices;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
  [Route("api/resources")]
  public class SystemResourcesController : ControllerBase
  {
    private readonly CreateSystemResource _createSystemResource;
    private readonly GetAllSystemResources _getAllSystemResources;
    private readonly GetSystemResourceById _getSystemResourceById;
    private readonly UpdateSystemResource _updateSystemResource;
    private readonly DeleteSystemResource _deleteSystemResource;
    private readonly SearchSystemResources _searchSystemResources;

    public SystemResourcesController(
        CreateSystemResource createSystemResource,
        GetAllSystemResources getAllSystemResources,
        GetSystemResourceById getSystemResourceById,
        UpdateSystemResource updateSystemResource,
        DeleteSystemResource deleteSystemResource,
        SearchSystemResources searchSystemResources)
    {
      _createSystemResource = createSystemResource;
      _getAllSystemResources = getAllSystemResources;
      _getSystemResourceById = getSystemResourceById;
      _updateSystemResource = updateSystemResource;
      _deleteSystemResource = deleteSystemResource;
      _searchSystemResources = searchSystemResources;
    }

    // POST: api/resources
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SystemResourceCreateDto dto)
    {
      if (dto == null) return BadRequest(new { message = "Payload inválido." });

      var created = await _createSystemResource.ExecuteAsync(dto);
      if (created == null)
        return BadRequest(new { message = "Falha ao criar recurso do sistema." });

      return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // GET: api/resources?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
      var allResources = await _getAllSystemResources.ExecuteAsync(page, pageSize);
      return Ok(allResources);
    }

    // GET: api/resources/options
    [HttpGet("options")]
    public async Task<IActionResult> GetOptions()
    {
      var options = await _getAllSystemResources.GetOptionsAsync();
      return Ok(options);
    }

    // GET: api/resources/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
      var resource = await _getSystemResourceById.ExecuteAsync(id);
      if (resource == null) return NotFound(new { message = "Recurso do sistema não encontrado." });
      return Ok(resource);
    }

    // PUT: api/resources/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SystemResourceUpdateDto dto)
    {
      if (dto == null) return BadRequest(new { message = "Payload inválido." });

      var updated = await _updateSystemResource.ExecuteAsync(id, dto);
      if (updated == null) return NotFound(new { message = "Recurso do sistema não encontrado." });

      return Ok(updated);
    }

    // DELETE: api/resources/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
      var deleted = await _deleteSystemResource.ExecuteAsync(id);
      if (!deleted) return NotFound(new { message = "Recurso do sistema não encontrado." });
      return NoContent();
    }

    // GET: api/resources/search?key=abc&page=1&pageSize=10
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string key, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
      if (string.IsNullOrWhiteSpace(key))
        return BadRequest(new { message = "A chave de pesquisa é obrigatória." });

      var foundResources = await _searchSystemResources.ExecuteAsync(key, page, pageSize);
      return Ok(foundResources);
    }
  }
}
