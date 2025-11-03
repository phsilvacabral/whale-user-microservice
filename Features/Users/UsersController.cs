using Microsoft.AspNetCore.Mvc;
using users_service.Models;
using users_service.Services;

namespace users_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Busca um usuário por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { error = "Usuário não encontrado", message = "O usuário solicitado não existe" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário {UserId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Busca um usuário por email
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { error = "Usuário não encontrado", message = "O usuário solicitado não existe" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por email {Email}", email);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _userService.UpdateAsync(id, request);
                if (user == null)
                {
                    return NotFound(new { error = "Usuário não encontrado", message = "O usuário solicitado não existe" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário {UserId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Deleta um usuário (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                var deleted = await _userService.DeleteAsync(id);
                if (!deleted)
                {
                    return NotFound(new { error = "Usuário não encontrado", message = "O usuário solicitado não existe" });
                }

                return Ok(new { message = "Usuário deletado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário {UserId}", id);
                return StatusCode(500, new { error = "Erro interno do servidor" });
            }
        }
    }
}
