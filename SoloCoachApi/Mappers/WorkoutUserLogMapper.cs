using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class WorkoutUserLogMapper
    {
        public WorkoutUserLog ToModel(WorkoutUserLogDto dto)
        {
            return new WorkoutUserLog
            {
                IdWorkoutUserLog = dto.IdWorkoutUserLog,
                WorkoutUserId = dto.WorkoutUserId,
                WorkoutId = dto.WorkoutId,
                Repetitions = dto.Repetitions,
                Sets = dto.Sets,
                Weight = dto.Weight,
                Status = dto.Status
            };
        }

        public WorkoutUserLogDto ToDto(WorkoutUserLog workoutUserLog)
        {
            return new WorkoutUserLogDto
            {
                IdWorkoutUserLog = workoutUserLog.IdWorkoutUserLog,
                WorkoutUserId = workoutUserLog.WorkoutUserId,
                WorkoutId = workoutUserLog.WorkoutId,
                Repetitions = workoutUserLog.Repetitions,
                Sets = workoutUserLog.Sets,
                Weight = workoutUserLog.Weight,
                Status = workoutUserLog.Status
            };
        }
    }
}
