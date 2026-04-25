using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanWorkoutController : ControllerBase
    {
        private readonly PlanWorkoutService _planWorkoutService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<PlanWorkoutController> _logger;

        public PlanWorkoutController(PlanWorkoutService planWorkoutService, ILoggingService loggingService, ILogger<PlanWorkoutController> logger)
        {
            _planWorkoutService = planWorkoutService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlanWorkoutDto>> GetById(int id)
        {
            var plan = await _planWorkoutService.GetByIdAsync(id);
            return Ok(plan);
        }

        [HttpGet]
        public async Task<ActionResult<List<PlanWorkoutDto>>> GetAll()
        {
            var plans = await _planWorkoutService.GetAllAsync();
            return Ok(plans);
        }

        [HttpPost]
        public async Task<ActionResult<PlanWorkoutDto>> Create(PlanWorkoutDto dto)
        {
            try
            {
                var created = await _planWorkoutService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_PLAN_WORKOUT", entityType: "PlanWorkout", entityId: created.IdPlanWorkout, details: new { status = created.Status });
                return CreatedAtAction(nameof(GetById), new { id = created.IdPlanWorkout }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plan workout");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_PLAN_WORKOUT", entityType: "PlanWorkout", entityId: null, details: new { status = dto.Status }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PlanWorkoutDto>> Update(int id, PlanWorkoutDto dto)
        {
            try
            {
                if (id != dto.IdPlanWorkout) return BadRequest("ID mismatch between route and body.");
                var updated = await _planWorkoutService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_PLAN_WORKOUT", entityType: "PlanWorkout", entityId: updated.IdPlanWorkout, details: new { status = updated.Status });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan workout");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_PLAN_WORKOUT", entityType: "PlanWorkout", entityId: id, details: new { status = dto.Status }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _planWorkoutService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_PLAN_WORKOUT", entityType: "PlanWorkout", entityId: id, details: new { deletedId = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting plan workout");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_PLAN_WORKOUT", entityType: "PlanWorkout", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }
    }
}

