using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutSessionController : ControllerBase
{
    private readonly WorkoutSessionService _workoutSessionService;
    private readonly ILoggingService _loggingService;
    private readonly ILogger<WorkoutSessionController> _logger;

    public WorkoutSessionController(WorkoutSessionService workoutSessionService, ILoggingService loggingService, ILogger<WorkoutSessionController> logger)
    {
        _workoutSessionService = workoutSessionService;
        _loggingService = loggingService;
        _logger = logger;
    }

    /// <summary>Создаёт запись workout_users и строки workout_user_logs в одной транзакции.</summary>
    [HttpPost("complete")]
    public async Task<ActionResult<CompleteWorkoutSessionResultDto>> Complete([FromBody] CompleteWorkoutSessionDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var result = await _workoutSessionService.CompleteAsync(userId, dto);
            await _loggingService.LogActionAsync(userId: userId, action: "COMPLETE_WORKOUT_SESSION", entityType: "WorkoutSession", entityId: null, details: new { workoutId = dto.WorkoutId });
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            await _loggingService.LogActionAsync(userId: userId, action: "COMPLETE_WORKOUT_SESSION", entityType: "WorkoutSession", entityId: null, details: new { workoutId = dto.WorkoutId }, status: "ERROR", errorMessage: ex.Message);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            await _loggingService.LogActionAsync(userId: userId, action: "COMPLETE_WORKOUT_SESSION", entityType: "WorkoutSession", entityId: null, details: new { workoutId = dto.WorkoutId }, status: "ERROR", errorMessage: ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
