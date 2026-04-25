using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly WorkoutService _workoutService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<WorkoutController> _logger;

        public WorkoutController(WorkoutService workoutService, ILoggingService loggingService, ILogger<WorkoutController> logger)
        {
            _workoutService = workoutService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<WorkoutDto>>> GetPaged([FromQuery] WorkoutListQuery query)
        {
            var result = await _workoutService.GetPagedAsync(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WorkoutDto>> GetById(int id)
        {
            var workout = await _workoutService.GetByIdAsync(id);
            return Ok(workout);
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkoutDto>>> GetAll()
        {
            var workouts = await _workoutService.GetAllAsync();
            return Ok(workouts);
        }

        [HttpPost]
        public async Task<ActionResult<WorkoutDto>> Create(WorkoutDto dto)
        {
            try
            {
                var created = await _workoutService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_WORKOUT",
                    entityType: "Workout",
                    entityId: created.IdWorkout,
                    details: new { name = created.Name, complexity = created.Complexity }
                );
                return CreatedAtAction(nameof(GetById), new { id = created.IdWorkout }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workout");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_WORKOUT",
                    entityType: "Workout",
                    entityId: null,
                    details: new { name = dto.Name },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<WorkoutDto>> Update(int id, WorkoutDto dto)
        {
            try
            {
                if (id != dto.IdWorkout)
                {
                    return BadRequest("ID mismatch between route and body.");
                }
                var updated = await _workoutService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_WORKOUT",
                    entityType: "Workout",
                    entityId: updated.IdWorkout,
                    details: new { name = updated.Name, complexity = updated.Complexity }
                );
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workout");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_WORKOUT",
                    entityType: "Workout",
                    entityId: id,
                    details: new { name = dto.Name },
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
                await _workoutService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "DELETE_WORKOUT",
                    entityType: "Workout",
                    entityId: id,
                    details: new { deletedWorkoutId = id }
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workout");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "DELETE_WORKOUT",
                    entityType: "Workout",
                    entityId: id,
                    details: new { deletedWorkoutId = id },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }
    }
}

