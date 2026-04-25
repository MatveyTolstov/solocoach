using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class MetricsUserMapper
    {
        public MetricsUser ToModel(MetricsUserDto dto)
        {
            return new MetricsUser
            {
                IdMetricsUser = dto.IdMetricsUser,
                Height = dto.Height,
                Weight = dto.Weight,
                Age = dto.Age,
                Gender = dto.Gender,
                ExperienceLevel = dto.ExperienceLevel,
                ActivityLevel = dto.ActivityLevel,
                GoalId = dto.GoalId ?? 0
            };
        }

        public MetricsUserDto ToDto(MetricsUser metricsUser)
        {
            return new MetricsUserDto
            {
                IdMetricsUser = metricsUser.IdMetricsUser,
                Height = metricsUser.Height,
                Weight = metricsUser.Weight,
                Age = metricsUser.Age,
                Gender = metricsUser.Gender,
                ExperienceLevel = metricsUser.ExperienceLevel,
                ActivityLevel = metricsUser.ActivityLevel,
                GoalId = metricsUser.GoalId
            };
        }
    }
}
