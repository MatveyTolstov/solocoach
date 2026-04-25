using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class GoalMapper
    {
        public Goal ToModel(GoalDto dto)
        {
            return new Goal
            {
                IdGoal = dto.IdGoal,
                TypeGoal = dto.TypeGoal,
                TargetWeight = dto.TargetWeight,
                DateStart = dto.DateStart,
                DateEnd = dto.DateEnd,
                Status = dto.Status
            };
        }

        public GoalDto ToDto(Goal goal)
        {
            return new GoalDto
            {
                IdGoal = goal.IdGoal,
                TypeGoal = goal.TypeGoal,
                TargetWeight = goal.TargetWeight,
                DateStart = goal.DateStart,
                DateEnd = goal.DateEnd,
                Status = goal.Status
            };
        }
    }
}
