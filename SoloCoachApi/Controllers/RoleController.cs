using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(RoleService roleService, ILoggingService loggingService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleDto>> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return Ok(role);
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create(RoleDto dto)
        {
            try
            {
                var created = await _roleService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_ROLE",
                    entityType: "Role",
                    entityId: created.IdRole,
                    details: new { roleName = created.RoleName }
                );
                return CreatedAtAction(nameof(GetById), new { id = created.IdRole }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_ROLE", entityType: "Role", entityId: null, details: new { roleName = dto.RoleName }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoleDto>> Update(int id, RoleDto dto)
        {
            try
            {
                if (id != dto.IdRole) return BadRequest("ID mismatch between route and body.");
                var updated = await _roleService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_ROLE", entityType: "Role", entityId: updated.IdRole, details: new { roleName = updated.RoleName });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_ROLE", entityType: "Role", entityId: id, details: new { roleName = dto.RoleName }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _roleService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_ROLE", entityType: "Role", entityId: id, details: new { deletedRoleId = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_ROLE", entityType: "Role", entityId: id, details: new { deletedRoleId = id }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }
    }
}

