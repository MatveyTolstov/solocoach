using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsMuscleController : ControllerBase
    {
        private readonly GroupsMuscleService _groupsMuscleService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<GroupsMuscleController> _logger;

        public GroupsMuscleController(GroupsMuscleService groupsMuscleService, ILoggingService loggingService, ILogger<GroupsMuscleController> logger)
        {
            _groupsMuscleService = groupsMuscleService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupsMuscleDto>> GetById(int id)
        {
            var group = await _groupsMuscleService.GetByIdAsync(id);
            return Ok(group);
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupsMuscleDto>>> GetAll()
        {
            var groups = await _groupsMuscleService.GetAllAsync();
            return Ok(groups);
        }

        [HttpPost]
        public async Task<ActionResult<GroupsMuscleDto>> Create(GroupsMuscleDto dto)
        {
            try
            {
                var created = await _groupsMuscleService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_GROUPS_MUSCLE", entityType: "GroupsMuscle", entityId: created.IdGroupsMuscle, details: new { name = created.Name });
                return CreatedAtAction(nameof(GetById), new { id = created.IdGroupsMuscle }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating groups muscle");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_GROUPS_MUSCLE", entityType: "GroupsMuscle", entityId: null, details: new { name = dto.Name }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GroupsMuscleDto>> Update(int id, GroupsMuscleDto dto)
        {
            try
            {
                if (id != dto.IdGroupsMuscle) return BadRequest("ID mismatch between route and body.");
                var updated = await _groupsMuscleService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_GROUPS_MUSCLE", entityType: "GroupsMuscle", entityId: updated.IdGroupsMuscle, details: new { name = updated.Name });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating groups muscle");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_GROUPS_MUSCLE", entityType: "GroupsMuscle", entityId: id, details: new { name = dto.Name }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _groupsMuscleService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_GROUPS_MUSCLE", entityType: "GroupsMuscle", entityId: id, details: new { deletedId = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting groups muscle");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_GROUPS_MUSCLE", entityType: "GroupsMuscle", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }
    }
}

