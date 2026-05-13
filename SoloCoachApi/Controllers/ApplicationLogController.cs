namespace SoloCoachApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SoloCoachApi.Extensions;
    using SoloCoachApi.Services;
    using SoloCoachApi.Mappers;
    using Microsoft.EntityFrameworkCore;
    using SoloCoachApi.DataBase;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationLogController : ControllerBase
    {
        private readonly ApplicationLogService _logService;
        private readonly ApplicationLogMapper _logMapper;
        private readonly ApplicationContext _context;
        private readonly ILogger<ApplicationLogController> _logger;

        public ApplicationLogController(ApplicationLogService logService, ApplicationLogMapper logMapper, ApplicationContext context, ILogger<ApplicationLogController> logger)
        {
            _logService = logService;
            _logMapper = logMapper;
            _context = context;
            _logger = logger;
        }

        [HttpPost("delete-all")]
        public async Task<IActionResult> DeleteAllLogs()
        {
            try
            {
                await _logService.DeleteAllLogsAsync();
                return Ok(new { message = "Все логи приложения удалены." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _logService.GetAllLogsAsync();
                return Ok(logs.Select(l => _logMapper.ToDto(l)));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] int? userId = null,
            [FromQuery] string? action = null,
            [FromQuery] string? entityType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            try
            {
                var query = _context.ApplicationLogs.AsQueryable();

                if (userId.HasValue)
                    query = query.Where(x => x.UserId == userId);

                if (!string.IsNullOrEmpty(action))
                    query = query.Where(x => x.Action.Contains(action));

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(x => x.EntityType.Contains(entityType));

                if (startDate.HasValue)
                    query = query.Where(x => x.CreatedAt >= startDate);

                if (endDate.HasValue)
                    query = query.Where(x => x.CreatedAt <= endDate);

                var logs = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                return Ok(logs.Select(l => _logMapper.ToDto(l)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-action/{action}")]
        public async Task<IActionResult> GetLogsByAction(string action, [FromQuery] int take = 100)
        {
            try
            {
                var logs = await _context.ApplicationLogs
                    .Where(x => x.Action == action)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(take)
                    .ToListAsync();

                return Ok(logs.Select(l => _logMapper.ToDto(l)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs by action");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-entity/{entityType}/{entityId}")]
        public async Task<IActionResult> GetLogsByEntity(string entityType, int entityId)
        {
            try
            {
                var logs = await _context.ApplicationLogs
                    .Where(x => x.EntityType == entityType && x.EntityId == entityId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return Ok(logs.Select(l => _logMapper.ToDto(l)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs by entity");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetLogsByUser(int userId, [FromQuery] int take = 100)
        {
            try
            {
                var logs = await _context.ApplicationLogs
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(take)
                    .ToListAsync();

                return Ok(logs.Select(l => _logMapper.ToDto(l)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs by user");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("errors")]
        public async Task<IActionResult> GetErrorLogs([FromQuery] int skip = 0, [FromQuery] int take = 50)
        {
            try
            {
                var logs = await _context.ApplicationLogs
                    .Where(x => x.Status == "ERROR")
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                return Ok(logs.Select(l => _logMapper.ToDto(l)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting error logs");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetLogsSummary()
        {
            try
            {
                var totalLogs = await _context.ApplicationLogs.CountAsync();
                var errorCount = await _context.ApplicationLogs.CountAsync(x => x.Status == "ERROR");
                var successCount = await _context.ApplicationLogs.CountAsync(x => x.Status == "SUCCESS");

                var actionsSummary = await _context.ApplicationLogs
                    .GroupBy(x => x.Action)
                    .Select(g => new { action = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .ToListAsync();

                var entitiesSummary = await _context.ApplicationLogs
                    .GroupBy(x => x.EntityType)
                    .Select(g => new { entityType = g.Key, count = g.Count() })
                    .OrderByDescending(x => x.count)
                    .ToListAsync();

                return Ok(new
                {
                    totalLogs,
                    errorCount,
                    successCount,
                    successRate = totalLogs > 0 ? (double)successCount / totalLogs * 100 : 0,
                    actionsSummary,
                    entitiesSummary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting logs summary");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
