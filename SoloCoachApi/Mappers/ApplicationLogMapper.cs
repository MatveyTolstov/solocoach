namespace SoloCoachApi.Mappers
{
    using SoloCoachApi.Models;
    using SoloCoachApi.ModelDto;

    public class ApplicationLogMapper
    {
        public ApplicationLog ToModel(ApplicationLogDto dto)
        {
            return new ApplicationLog
            {
                IdApplicationLog = dto.IdApplicationLog,
                Action = dto.Action,
                EntityType = dto.EntityType,
                EntityId = dto.EntityId,
                UserId = dto.UserId,
                Details = dto.Details,
                CreatedAt = dto.CreatedAt,
                Status = dto.Status,
                ErrorMessage = dto.ErrorMessage,
            };
        }

        public ApplicationLogDto ToDto(ApplicationLog log)
        {
            return new ApplicationLogDto
            {
                IdApplicationLog = log.IdApplicationLog,
                Action = log.Action,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                UserId = log.UserId,
                Details = log.Details,
                CreatedAt = log.CreatedAt,
                Status = log.Status,
                ErrorMessage = log.ErrorMessage,
            };
        }
    }
}
