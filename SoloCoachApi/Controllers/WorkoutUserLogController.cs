using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutUserLogController : ControllerBase
{
    private readonly WorkoutUserLogService _workoutUserLogService;
    private readonly ILoggingService _loggingService;
    private readonly ILogger<WorkoutUserLogController> _logger;

    public WorkoutUserLogController(WorkoutUserLogService workoutUserLogService, ILoggingService loggingService, ILogger<WorkoutUserLogController> logger)
    {
        _workoutUserLogService = workoutUserLogService;
        _loggingService = loggingService;
        _logger = logger;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<List<WorkoutUserLogDto>>> GetAllForAdmin()
    {
        var logs = await _workoutUserLogService.GetAllAsync();
        return Ok(logs);
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutUserLogDto>> GetByIdForAdmin(int id)
    {
        var log = await _workoutUserLogService.GetByIdAsync(id);
        return Ok(log);
    }

    [HttpPost("admin")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutUserLogDto>> CreateForAdmin([FromBody] WorkoutUserLogDto dto)
    {
        try
        {
            var created = await _workoutUserLogService.CreateAsync(dto);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "CREATE_WORKOUT_USER_LOG", entityType: "WorkoutUserLog", entityId: created.IdWorkoutUserLog, details: new { workoutUserId = created.WorkoutUserId });
            return CreatedAtAction(nameof(GetByIdForAdmin), new { id = created.IdWorkoutUserLog }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workout user log");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "CREATE_WORKOUT_USER_LOG", entityType: "WorkoutUserLog", entityId: null, details: new { workoutUserId = dto.WorkoutUserId }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }

    [HttpPut("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutUserLogDto>> UpdateForAdmin(int id, [FromBody] WorkoutUserLogDto dto)
    {
        try
        {
            if (id != dto.IdWorkoutUserLog) return BadRequest("Несоответствие ID между маршрутом и телом сообщения.");
            var updated = await _workoutUserLogService.UpdateAsync(dto);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_WORKOUT_USER_LOG", entityType: "WorkoutUserLog", entityId: updated.IdWorkoutUserLog, details: new { workoutUserId = updated.WorkoutUserId });
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workout user log");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_WORKOUT_USER_LOG", entityType: "WorkoutUserLog", entityId: id, details: new { workoutUserId = dto.WorkoutUserId }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }

    [HttpDelete("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<IActionResult> DeleteForAdmin(int id)
    {
        try
        {
            await _workoutUserLogService.DeleteAsync(id);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "DELETE_WORKOUT_USER_LOG", entityType: "WorkoutUserLog", entityId: id, details: new { deletedId = id });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workout user log");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "DELETE_WORKOUT_USER_LOG", entityType: "WorkoutUserLog", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }
    [HttpGet("me/session/{workoutUserId:int}")]
    public async Task<ActionResult<List<WorkoutUserLogDto>>> GetForSession(int workoutUserId)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var logs = await _workoutUserLogService.GetByWorkoutUserForUserAsync(workoutUserId, userId);
            return Ok(logs);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutUserLogDto>> GetById(int id)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var log = await _workoutUserLogService.GetByIdAsync(id);
        try
        {
            await _workoutUserLogService.GetByWorkoutUserForUserAsync(log.WorkoutUserId, userId);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return Ok(log);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutUserLogDto>> Create([FromBody] WorkoutUserLogDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        try
        {
            await _workoutUserLogService.GetByWorkoutUserForUserAsync(dto.WorkoutUserId, userId);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        var created = await _workoutUserLogService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.IdWorkoutUserLog }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkoutUserLogDto>> Update(int id, [FromBody] WorkoutUserLogDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (id != dto.IdWorkoutUserLog)
        {
            return BadRequest("Несоответствие ID между маршрутом и телом сообщения.");
        }

        try
        {
            await _workoutUserLogService.GetByWorkoutUserForUserAsync(dto.WorkoutUserId, userId);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        var updated = await _workoutUserLogService.UpdateAsync(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var existing = await _workoutUserLogService.GetByIdAsync(id);
        try
        {
            await _workoutUserLogService.GetByWorkoutUserForUserAsync(existing.WorkoutUserId, userId);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        await _workoutUserLogService.DeleteAsync(id);
        return NoContent();
    }
}
