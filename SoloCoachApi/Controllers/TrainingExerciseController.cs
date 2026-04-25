using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingExerciseController : ControllerBase
    {
        private readonly TrainingExerciseService _trainingExerciseService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<TrainingExerciseController> _logger;

        public TrainingExerciseController(TrainingExerciseService trainingExerciseService, ILoggingService loggingService, ILogger<TrainingExerciseController> logger)
        {
            _trainingExerciseService = trainingExerciseService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TrainingExerciseDto>> GetById(int id)
        {
            var trainingExercise = await _trainingExerciseService.GetByIdAsync(id);
            return Ok(trainingExercise);
        }

        [HttpGet]
        public async Task<ActionResult<List<TrainingExerciseDto>>> GetAll()
        {
            var trainingExercises = await _trainingExerciseService.GetAllAsync();
            return Ok(trainingExercises);
        }

        [HttpPost]
        public async Task<ActionResult<TrainingExerciseDto>> Create(TrainingExerciseDto dto)
        {
            try
            {
                var created = await _trainingExerciseService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_TRAINING_EXERCISE", entityType: "TrainingExercise", entityId: created.IdTrainingExercise, details: new { exerciseId = created.ExerciseId });
                return CreatedAtAction(nameof(GetById), new { id = created.IdTrainingExercise }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating training exercise");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_TRAINING_EXERCISE", entityType: "TrainingExercise", entityId: null, details: new { exerciseId = dto.ExerciseId }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TrainingExerciseDto>> Update(int id, TrainingExerciseDto dto)
        {
            try
            {
                if (id != dto.IdTrainingExercise) return BadRequest("ID mismatch between route and body.");
                var updated = await _trainingExerciseService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_TRAINING_EXERCISE", entityType: "TrainingExercise", entityId: updated.IdTrainingExercise, details: new { exerciseId = updated.ExerciseId });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training exercise");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_TRAINING_EXERCISE", entityType: "TrainingExercise", entityId: id, details: new { exerciseId = dto.ExerciseId }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _trainingExerciseService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_TRAINING_EXERCISE", entityType: "TrainingExercise", entityId: id, details: new { deletedId = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting training exercise");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_TRAINING_EXERCISE", entityType: "TrainingExercise", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }
    }
}

