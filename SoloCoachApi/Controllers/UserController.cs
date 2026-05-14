using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILoggingService loggingService, ILogger<UserController> logger)
        {
            _userService = userService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserDto dto)
        {
            try
            {
                var created = await _userService.CreateAsync(dto);
                
                // Логируем создание пользователя
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_USER",
                    entityType: "User",
                    entityId: created.IdUser,
                    details: new { login = created.Login, name = created.Name }
                );
                
                return CreatedAtAction(nameof(GetById), new { id = created.IdUser }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_USER",
                    entityType: "User",
                    entityId: null,
                    details: new { login = dto.Login },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDto>> Update(int id, UserDto dto)
        {
            try
            {
                if (id != dto.IdUser)
                {
                    return BadRequest("ID mismatch between route and body.");
                }

                var updated = await _userService.UpdateAsync(dto);
                
                // Логируем обновление пользователя
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_USER",
                    entityType: "User",
                    entityId: updated.IdUser,
                    details: new { login = updated.Login, name = updated.Name }
                );
                
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_USER",
                    entityType: "User",
                    entityId: id,
                    details: new { login = dto.Login },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = this.User.GetUserId();
            if (id == currentUserId)
                return BadRequest("Нельзя удалить самого себя.");

            try
            {
                await _userService.DeleteAsync(id);
                
                await _loggingService.LogActionAsync(
                    userId: currentUserId,
                    action: "DELETE_USER",
                    entityType: "User",
                    entityId: id,
                    details: new { deletedUserId = id }
                );
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                await _loggingService.LogActionAsync(
                    userId: currentUserId,
                    action: "DELETE_USER",
                    entityType: "User",
                    entityId: id,
                    details: new { deletedUserId = id },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [Authorize]
        [HttpPost("request-email-change")]
        public async Task<IActionResult> RequestEmailChange([FromBody] RequestEmailChangeDto request)
        {
            var userId = this.User.GetUserId();
            await _userService.SendEmailCode(userId, request);
            return Ok("Код отправлен на новый email");
        }

        [Authorize]
        [HttpPost("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeDto request)
        {
            var userId = this.User.GetUserId();
            await _userService.ConfirmEmailChange(userId, request);
            return Ok("Email успешно изменён");
        }
    }
}

