using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutCalendarController : ControllerBase
{
    private readonly WorkoutCalendarService _workoutCalendarService;
    private readonly ILoggingService _loggingService;
    private readonly ILogger<WorkoutCalendarController> _logger;

    public WorkoutCalendarController(WorkoutCalendarService workoutCalendarService, ILoggingService loggingService, ILogger<WorkoutCalendarController> logger)
    {
        _workoutCalendarService = workoutCalendarService;
        _loggingService = loggingService;
        _logger = logger;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<List<WorkoutCalendarDto>>> GetAllForAdmin()
    {
        var calendars = await _workoutCalendarService.GetAllAsync();
        return Ok(calendars);
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutCalendarDto>> GetByIdForAdmin(int id)
    {
        var calendar = await _workoutCalendarService.GetByIdAsync(id);
        return Ok(calendar);
    }

    [HttpPost("admin")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutCalendarDto>> CreateForAdmin([FromBody] WorkoutCalendarDto dto)
    {
        try
        {
            var created = await _workoutCalendarService.CreateAsync(dto);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "CREATE_WORKOUT_CALENDAR", entityType: "WorkoutCalendar", entityId: created.IdWorkoutCalendar, details: new { name = created.WorkoutId });
            return CreatedAtAction(nameof(GetByIdForAdmin), new { id = created.IdWorkoutCalendar }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workout calendar");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "CREATE_WORKOUT_CALENDAR", entityType: "WorkoutCalendar", entityId: null, details: new { workoutId = dto.WorkoutId }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }

    [HttpPut("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutCalendarDto>> UpdateForAdmin(int id, [FromBody] WorkoutCalendarDto dto)
    {
        try
        {
            if (id != dto.IdWorkoutCalendar) return BadRequest("ID mismatch between route and body.");
            var updated = await _workoutCalendarService.UpdateAsync(dto);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_WORKOUT_CALENDAR", entityType: "WorkoutCalendar", entityId: updated.IdWorkoutCalendar, details: new { workoutId = updated.WorkoutId });
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workout calendar");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_WORKOUT_CALENDAR", entityType: "WorkoutCalendar", entityId: id, details: new { workoutId = dto.WorkoutId }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }

    [HttpDelete("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<IActionResult> DeleteForAdmin(int id)
    {
        try
        {
            await _workoutCalendarService.DeleteAsync(id);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "DELETE_WORKOUT_CALENDAR", entityType: "WorkoutCalendar", entityId: id, details: new { deletedId = id });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workout calendar");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "DELETE_WORKOUT_CALENDAR", entityType: "WorkoutCalendar", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }
    [HttpGet("me")]
    public async Task<ActionResult<List<WorkoutCalendarDto>>> GetMine(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var calendars = await _workoutCalendarService.GetByUserIdAsync(userId, from, to);
        return Ok(calendars);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutCalendarDto>> GetById(int id)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var calendar = await _workoutCalendarService.GetByIdAsync(id);
        if (calendar.UserId != userId)
        {
            return Forbid();
        }

        return Ok(calendar);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutCalendarDto>> Create([FromBody] WorkoutCalendarDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        dto.UserId = userId;
        if (string.IsNullOrWhiteSpace(dto.Status))
        {
            dto.Status = global::SoloCoachApi.WorkoutCalendarStatus.Planned;
        }

        var created = await _workoutCalendarService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.IdWorkoutCalendar }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkoutCalendarDto>> Update(int id, [FromBody] WorkoutCalendarDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (id != dto.IdWorkoutCalendar)
        {
            return BadRequest("Несоответствие ID между маршрутом и телом сообщения.");
        }

        var existing = await _workoutCalendarService.GetByIdAsync(id);
        if (existing.UserId != userId)
        {
            return Forbid();
        }

        dto.UserId = userId;
        var updated = await _workoutCalendarService.UpdateAsync(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var existing = await _workoutCalendarService.GetByIdAsync(id);
        if (existing.UserId != userId)
        {
            return Forbid();
        }

        await _workoutCalendarService.DeleteAsync(id);
        return NoContent();
    }
}
