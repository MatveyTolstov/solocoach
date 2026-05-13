using System.Text.Json;
using SoloCoachApi.DataBase;
using SoloCoachApi.Models;

namespace SoloCoachApi.Services
{
    public interface ILoggingService
    {
        Task LogActionAsync(int? userId, string action, string entityType, int? entityId, object? details, string status = "SUCCESS", string? errorMessage = null);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ApplicationContext context, ILogger<LoggingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task DeleteAllLogsAsync()
        {
            var allLogs = _context.ApplicationLogs.ToList();
            _context.ApplicationLogs.RemoveRange(allLogs);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Все логи приложения удалены.");
        }
        public async Task LogActionAsync(int? userId, string action, string entityType, int? entityId, object? details, string status = "SUCCESS", string? errorMessage = null)
        {
            try
            {
                var detailsJson = details != null ? JsonSerializer.Serialize(details) : "{}";

                var log = new ApplicationLog
                {
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    UserId = userId,
                    Details = detailsJson,
                    CreatedAt = DateTime.UtcNow,
                    Status = status,
                    ErrorMessage = errorMessage
                };

                _context.ApplicationLogs.Add(log);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Action logged: {action} on {entityType} (ID: {entityId}) by User {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging action");
                // Не прерываем основной процесс если логирование не сработало
            }
        }
    }
}
