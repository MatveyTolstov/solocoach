using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IWorkoutRepository
    {
        Task<WorkoutDto> GetWorkoutByIdAsync(int id);
        Task<List<WorkoutDto>> GetAllWorkoutsAsync();
        Task<PagedResultDto<WorkoutDto>> GetWorkoutsPagedAsync(int page, int pageSize, string? search, string? typeWorkout, string? complexity);
        Task<WorkoutDto> CreateWorkoutAsync(WorkoutDto dto);
        Task<WorkoutDto> UpdateWorkoutAsync(WorkoutDto dto);
        Task DeleteWorkoutAsync(int id);
    }
}

