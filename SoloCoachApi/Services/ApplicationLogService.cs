namespace SoloCoachApi.Services
{
    using System.Text.Json;
    using SoloCoachApi.Models;
    using SoloCoachApi.Repositories;

    public class ApplicationLogService
    {
        private readonly IApplicationLogRepository _logRepository;

        public ApplicationLogService(IApplicationLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task LogActionAsync(
            string action,
            string entityType,
            int? entityId = null,
            int? userId = null,
            object? details = null,
            string? status = "SUCCESS",
            string? errorMessage = null
        )
        {
            var detailsJson = details != null 
                ? JsonSerializer.Serialize(details) 
                : "{}";

            var log = new ApplicationLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Details = detailsJson,
                CreatedAt = DateTime.UtcNow,
                Status = status,
                ErrorMessage = errorMessage,
            };

            await _logRepository.CreateAsync(log);
        }

        public async Task<List<ApplicationLog>> GetAllLogsAsync()
        {
            return await _logRepository.GetAllAsync();
        }
    }
}
