using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly GoalService _goalService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<GoalController> _logger;

        public GoalController(GoalService goalService, ILoggingService loggingService, ILogger<GoalController> logger)
        {
            _goalService = goalService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GoalDto>> GetById(int id)
        {
            var goal = await _goalService.GetByIdAsync(id);
            return Ok(goal);
        }

        [HttpGet]
        public async Task<ActionResult<List<GoalDto>>> GetAll()
        {
            var goals = await _goalService.GetAllAsync();
            return Ok(goals);
        }

        [HttpPost]
        public async Task<ActionResult<GoalDto>> Create(GoalDto dto)
        {
            try
            {
                var created = await _goalService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_GOAL",
                    entityType: "Goal",
                    entityId: created.IdGoal,
                    details: new { typeGoal = created.TypeGoal, targetWeight = created.TargetWeight }
                );
                return CreatedAtAction(nameof(GetById), new { id = created.IdGoal }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_GOAL",
                    entityType: "Goal",
                    entityId: null,
                    details: new { typeGoal = dto.TypeGoal },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GoalDto>> Update(int id, GoalDto dto)
        {
            try
            {
                if (id != dto.IdGoal)
                {
                    return BadRequest("ID mismatch between route and body.");
                }
                var updated = await _goalService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_GOAL",
                    entityType: "Goal",
                    entityId: updated.IdGoal,
                    details: new { typeGoal = updated.TypeGoal, targetWeight = updated.TargetWeight }
                );
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_GOAL",
                    entityType: "Goal",
                    entityId: id,
                    details: new { typeGoal = dto.TypeGoal },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _goalService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "DELETE_GOAL",
                    entityType: "Goal",
                    entityId: id,
                    details: new { deletedGoalId = id }
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting goal");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "DELETE_GOAL",
                    entityType: "Goal",
                    entityId: id,
                    details: new { deletedGoalId = id },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }
    }
}

