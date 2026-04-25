using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class WorkoutCalendarMapper
    {
        public WorkoutCalendar ToModel(WorkoutCalendarDto dto)
        {
            return new WorkoutCalendar
            {
                IdWorkoutCalendar = dto.IdWorkoutCalendar,
                UserId = dto.UserId,
                WorkoutId = dto.WorkoutId,
                Status = dto.Status,
                Date = dto.Date
            };
        }

        public WorkoutCalendarDto ToDto(WorkoutCalendar workoutCalendar)
        {
            return new WorkoutCalendarDto
            {
                IdWorkoutCalendar = workoutCalendar.IdWorkoutCalendar,
                UserId = workoutCalendar.UserId,
                WorkoutId = workoutCalendar.WorkoutId,
                Status = workoutCalendar.Status,
                Date = workoutCalendar.Date
            };
        }
    }
}
