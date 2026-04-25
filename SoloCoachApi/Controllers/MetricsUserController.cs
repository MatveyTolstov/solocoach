using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsUserController : ControllerBase
    {
        private readonly MetricsUserService _metricsUserService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<MetricsUserController> _logger;

        public MetricsUserController(MetricsUserService metricsUserService, ILoggingService loggingService, ILogger<MetricsUserController> logger)
        {
            _metricsUserService = metricsUserService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MetricsUserDto>> GetById(int id)
        {
            var metrics = await _metricsUserService.GetByIdAsync(id);
            return Ok(metrics);
        }

        [HttpGet]
        public async Task<ActionResult<List<MetricsUserDto>>> GetAll()
        {
            var metrics = await _metricsUserService.GetAllAsync();
            return Ok(metrics);
        }

        [HttpPost]
        public async Task<ActionResult<MetricsUserDto>> Create(MetricsUserDto dto)
        {
            try
            {
                var created = await _metricsUserService.CreateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_METRICS_USER", entityType: "MetricsUser", entityId: created.IdMetricsUser, details: new { height = created.Height, weight = created.Weight });
                return CreatedAtAction(nameof(GetById), new { id = created.IdMetricsUser }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating metrics user");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "CREATE_METRICS_USER", entityType: "MetricsUser", entityId: null, details: new { height = dto.Height }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MetricsUserDto>> Update(int id, MetricsUserDto dto)
        {
            try
            {
                if (id != dto.IdMetricsUser) return BadRequest("ID mismatch between route and body.");
                var updated = await _metricsUserService.UpdateAsync(dto);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_METRICS_USER", entityType: "MetricsUser", entityId: updated.IdMetricsUser, details: new { height = updated.Height, weight = updated.Weight });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating metrics user");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_METRICS_USER", entityType: "MetricsUser", entityId: id, details: new { height = dto.Height }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _metricsUserService.DeleteAsync(id);
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_METRICS_USER", entityType: "MetricsUser", entityId: id, details: new { deletedId = id });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting metrics user");
                var userId = this.User.GetUserId();
                await _loggingService.LogActionAsync(userId: userId, action: "DELETE_METRICS_USER", entityType: "MetricsUser", entityId: id, details: new { deletedId = id }, status: "ERROR", errorMessage: ex.Message);
                throw;
            }
        }
    }
}

