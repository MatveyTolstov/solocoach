using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IWorkoutCalendarRepository
    {
        Task<WorkoutCalendarDto> GetWorkoutCalendarByIdAsync(int id);
        Task<List<WorkoutCalendarDto>> GetAllWorkoutCalendarsAsync();
        Task<List<WorkoutCalendarDto>> GetByUserIdAsync(int userId, DateTime? fromUtc, DateTime? toUtc);
        Task<WorkoutCalendarDto> CreateWorkoutCalendarAsync(WorkoutCalendarDto dto);
        Task<WorkoutCalendarDto> UpdateWorkoutCalendarAsync(WorkoutCalendarDto dto);
        Task DeleteWorkoutCalendarAsync(int id);
    }
}

