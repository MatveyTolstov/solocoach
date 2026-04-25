using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseGroupsMuscleController : ControllerBase
    {
        private readonly ExerciseGroupsMuscleService _exerciseGroupsMuscleService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<ExerciseGroupsMuscleController> _logger;

        public ExerciseGroupsMuscleController(ExerciseGroupsMuscleService exerciseGroupsMuscleService, ILoggingService loggingService, ILogger<ExerciseGroupsMuscleController> logger)
        {
            _exerciseGroupsMuscleService = exerciseGroupsMuscleService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExerciseGroupsMuscleDto>> GetById(int id)
        {
            var entity = await _exerciseGroupsMuscleService.GetByIdAsync(id);
            return Ok(entity);
        }

        [HttpGet]
        public async Task<ActionResult<List<ExerciseGroupsMuscleDto>>> GetAll()
        {
            var entities = await _exerciseGroupsMuscleService.GetAllAsync();
            return Ok(entities);
        }

        [HttpPost]
        public async Task<ActionResult<ExerciseGroupsMuscleDto>> Create(ExerciseGroupsMuscleDto dto)
        {
            try
            {
                var created = await _exerciseGroupsMuscleService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_EXERCISE_GROUPS_MUSCLE", entityType: "ExerciseGroupsMuscle", entityId: created.IdExerciseGroupsMuscle, details: new { exerciseId = created.ExerciseId, muscleGroupId = created.GroupsMusclesId });
                return CreatedAtAction(nameof(GetById), new { id = created.IdExerciseGroupsMuscle }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exercise groups muscle");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_EXERCISE_GROUPS_MUSCLE", entityType: "ExerciseGroupsMuscle", entityId: null, details: new { exerciseId = dto.ExerciseId }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExerciseGroupsMuscleDto>> Update(int id, ExerciseGroupsMuscleDto dto)
        {
            try
            {
                if (id != dto.IdExerciseGroupsMuscle) return BadRequest("ID mismatch between route and body.");
                var updated = await _exerciseGroupsMuscleService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_EXERCISE_GROUPS_MUSCLE", entityType: "ExerciseGroupsMuscle", entityId: updated.IdExerciseGroupsMuscle, details: new { exerciseId = updated.ExerciseId, muscleGroupId = updated.GroupsMusclesId });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise groups muscle");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_EXERCISE_GROUPS_MUSCLE", entityType: "ExerciseGroupsMuscle", entityId: id, details: new { exerciseId = dto.ExerciseId }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _exerciseGroupsMuscleService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_EXERCISE_GROUPS_MUSCLE", entityType: "ExerciseGroupsMuscle", entityId: id, details: new { deletedId = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exercise groups muscle");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_EXERCISE_GROUPS_MUSCLE", entityType: "ExerciseGroupsMuscle", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }
    }
}

