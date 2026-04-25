using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseController : ControllerBase
    {
        private readonly ExerciseService _exerciseService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<ExerciseController> _logger;

        public ExerciseController(ExerciseService exerciseService, ILoggingService loggingService, ILogger<ExerciseController> logger)
        {
            _exerciseService = exerciseService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultDto<ExerciseDto>>> GetPaged([FromQuery] ExerciseListQuery query)
        {
            var result = await _exerciseService.GetPagedAsync(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExerciseDto>> GetById(int id)
        {
            var exercise = await _exerciseService.GetByIdAsync(id);
            return Ok(exercise);
        }

        [HttpGet]
        public async Task<ActionResult<List<ExerciseDto>>> GetAll()
        {
            var exercises = await _exerciseService.GetAllAsync();
            return Ok(exercises);
        }

        [HttpPost]
        public async Task<ActionResult<ExerciseDto>> Create(ExerciseDto dto)
        {
            try
            {
                var created = await _exerciseService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_EXERCISE",
                    entityType: "Exercise",
                    entityId: created.IdExercise,
                    details: new { name = created.Name, complexity = created.Complexity }
                );
                return CreatedAtAction(nameof(GetById), new { id = created.IdExercise }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exercise");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "CREATE_EXERCISE",
                    entityType: "Exercise",
                    entityId: null,
                    details: new { name = dto.Name },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExerciseDto>> Update(int id, ExerciseDto dto)
        {
            try
            {
                if (id != dto.IdExercise)
                {
                    return BadRequest("ID mismatch between route and body.");
                }
                var updated = await _exerciseService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_EXERCISE",
                    entityType: "Exercise",
                    entityId: updated.IdExercise,
                    details: new { name = updated.Name, complexity = updated.Complexity }
                );
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "UPDATE_EXERCISE",
                    entityType: "Exercise",
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
                await _exerciseService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "DELETE_EXERCISE",
                    entityType: "Exercise",
                    entityId: id,
                    details: new { deletedExerciseId = id }
                );
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exercise");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(
                    userId: userId,
                    action: "DELETE_EXERCISE",
                    entityType: "Exercise",
                    entityId: id,
                    details: new { deletedExerciseId = id },
                    status: "ERROR",
                    errorMessage: ex.Message
                );
                throw;
            }
        }
    }
}

