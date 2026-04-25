using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class PlanWorkoutMapper
    {
        public PlanWorkout ToModel(PlanWorkoutDto dto)
        {
            return new PlanWorkout
            {
                IdPlanWorkout = dto.IdPlanWorkout,
                UserId = dto.UserId,
                WorkoutId = dto.WorkoutId,
                Status = dto.Status,
                Source = dto.Source
            };
        }

        public PlanWorkoutDto ToDto(PlanWorkout planWorkout)
        {
            return new PlanWorkoutDto
            {
                IdPlanWorkout = planWorkout.IdPlanWorkout,
                UserId = planWorkout.UserId,
                WorkoutId = planWorkout.WorkoutId,
                Status = planWorkout.Status,
                Source = planWorkout.Source
            };
        }
    }
}
