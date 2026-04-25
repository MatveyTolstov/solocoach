using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IWorkoutUserLogRepository
    {
        Task<WorkoutUserLogDto> GetWorkoutUserLogByIdAsync(int id);
        Task<List<WorkoutUserLogDto>> GetAllWorkoutUserLogsAsync();
        Task<List<WorkoutUserLogDto>> GetByWorkoutUserIdAsync(int workoutUserId);
        Task<WorkoutUserLogDto> CreateWorkoutUserLogAsync(WorkoutUserLogDto dto);
        Task<WorkoutUserLogDto> UpdateWorkoutUserLogAsync(WorkoutUserLogDto dto);
        Task DeleteWorkoutUserLogAsync(int id);
    }
}

