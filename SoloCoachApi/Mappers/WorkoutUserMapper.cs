using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class WorkoutUserMapper
    {
        public WorkoutUser ToModel(WorkoutUserDto dto)
        {
            return new WorkoutUser
            {
                IdWorkoutUser = dto.IdWorkoutUser,
                UserId = dto.UserId,
                WorkoutId = dto.WorkoutId,
                Duration = dto.Duration,
                Date = dto.Date,
                Status = dto.Status
            };
        }

        public WorkoutUserDto ToDto(WorkoutUser workoutUser)
        {
            return new WorkoutUserDto
            {
                IdWorkoutUser = workoutUser.IdWorkoutUser,
                UserId = workoutUser.UserId,
                WorkoutId = workoutUser.WorkoutId,
                Duration = workoutUser.Duration,
                Date = workoutUser.Date,
                Status = workoutUser.Status
            };
        }
    }
}
