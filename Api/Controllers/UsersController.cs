using Api.Dtos;
using Api.Helpers;
using Api.Services.UsersServices;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly CreateUser _createUser;
        private readonly GetAllUsers _getAllUsers;
        private readonly GetUserById _getUserById;
        private readonly UpdateUser _updateUser;
        private readonly DeleteUser _deleteUser;
        private readonly SearchUsers _searchUsers;

        public UsersController(
            CreateUser createUser,
            GetAllUsers getAllUsers,
            GetUserById getUserById,
            UpdateUser updateUser,
            DeleteUser deleteUser,
            SearchUsers searchUsers)
        {
            _createUser = createUser;
            _getAllUsers = getAllUsers;
            _getUserById = getUserById;
            _updateUser = updateUser;
            _deleteUser = deleteUser;
            _searchUsers = searchUsers;
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Payload inválido." });

            var created = await _createUser.ExecuteAsync(dto);
            if (created == null)
                return BadRequest(new { message = "Falha ao criar usuário." });

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // GET: api/users?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var allUsers = await _getAllUsers.ExecuteAsync(page, pageSize);
            return Ok(allUsers);
        }

        // GET: api/users/options
        [HttpGet("options")]
        public async Task<IActionResult> GetOptions()
        {
            var options = await _getAllUsers.GetOptionsAsync();
            return Ok(options);
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _getUserById.ExecuteAsync(id);
            if (user == null) return NotFound(new { message = "Usuário não encontrado." });
            return Ok(user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Payload inválido." });

            var updated = await _updateUser.ExecuteAsync(id, dto);
            if (updated == null) return NotFound(new { message = "Usuário não encontrado." });

            return Ok(updated);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _deleteUser.ExecuteAsync(id);
            if (!deleted) return NotFound(new { message = "Usuário não encontrado." });
            return NoContent();
        }

        // GET: api/users/search?key=abc&page=1&pageSize=10
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string key, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest(new { message = "A chave de pesquisa é obrigatória." });

            var usersFound = await _searchUsers.ExecuteAsync(key, page, pageSize);
            return Ok(usersFound);
        }
    }
}
