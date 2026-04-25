using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutUserController : ControllerBase
{
    private readonly WorkoutUserService _workoutUserService;
    private readonly ILoggingService _loggingService;
    private readonly ILogger<WorkoutUserController> _logger;

    public WorkoutUserController(WorkoutUserService workoutUserService, ILoggingService loggingService, ILogger<WorkoutUserController> logger)
    {
        _workoutUserService = workoutUserService;
        _loggingService = loggingService;
        _logger = logger;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<List<WorkoutUserDto>>> GetAllForAdmin()
    {
        var list = await _workoutUserService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutUserDto>> GetByIdForAdmin(int id)
    {
        var workoutUser = await _workoutUserService.GetByIdAsync(id);
        return Ok(workoutUser);
    }

    [HttpPost("admin")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutUserDto>> CreateForAdmin([FromBody] WorkoutUserDto dto)
    {
        try
        {
            var created = await _workoutUserService.CreateAsync(dto);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "CREATE_WORKOUT_USER", entityType: "WorkoutUser", entityId: created.IdWorkoutUser, details: new { workoutId = created.WorkoutId, userId = created.UserId });
            return CreatedAtAction(nameof(GetByIdForAdmin), new { id = created.IdWorkoutUser }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workout user");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "CREATE_WORKOUT_USER", entityType: "WorkoutUser", entityId: null, details: new { workoutId = dto.WorkoutId }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }

    [HttpPut("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<ActionResult<WorkoutUserDto>> UpdateForAdmin(int id, [FromBody] WorkoutUserDto dto)
    {
        try
        {
            if (id != dto.IdWorkoutUser) return BadRequest("ID mismatch between route and body.");
            var updated = await _workoutUserService.UpdateAsync(dto);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_WORKOUT_USER", entityType: "WorkoutUser", entityId: updated.IdWorkoutUser, details: new { workoutId = updated.WorkoutId, userId = updated.UserId });
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workout user");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_WORKOUT_USER", entityType: "WorkoutUser", entityId: id, details: new { workoutId = dto.WorkoutId }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }

    [HttpDelete("admin/{id:int}")]
    [Authorize(Roles = "1")]
    public async Task<IActionResult> DeleteForAdmin(int id)
    {
        try
        {
            await _workoutUserService.DeleteAsync(id);
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "DELETE_WORKOUT_USER", entityType: "WorkoutUser", entityId: id, details: new { deletedId = id });
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workout user");
            var userId = this.User.GetUserId();
            await _loggingService.LogActionAsync(userId: userId, action: "DELETE_WORKOUT_USER", entityType: "WorkoutUser", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
            throw;
        }
    }
    [HttpGet("me")]
    public async Task<ActionResult<List<WorkoutUserDto>>> GetMine()
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var list = await _workoutUserService.GetByUserIdAsync(userId);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutUserDto>> GetById(int id)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var wu = await _workoutUserService.GetByIdAsync(id);
        if (wu.UserId != userId)
        {
            return Forbid();
        }

        return Ok(wu);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutUserDto>> Create([FromBody] WorkoutUserDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        dto.UserId = userId;
        var created = await _workoutUserService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.IdWorkoutUser }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkoutUserDto>> Update(int id, [FromBody] WorkoutUserDto dto)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        if (id != dto.IdWorkoutUser)
        {
            return BadRequest("ID mismatch between route and body.");
        }

        var existing = await _workoutUserService.GetByIdAsync(id);
        if (existing.UserId != userId)
        {
            return Forbid();
        }

        dto.UserId = userId;
        var updated = await _workoutUserService.UpdateAsync(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var existing = await _workoutUserService.GetByIdAsync(id);
        if (existing.UserId != userId)
        {
            return Forbid();
        }

        await _workoutUserService.DeleteAsync(id);
        return NoContent();
    }
}
