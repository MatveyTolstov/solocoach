using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IWorkoutUserRepository
    {
        Task<WorkoutUserDto> GetWorkoutUserByIdAsync(int id);
        Task<List<WorkoutUserDto>> GetAllWorkoutUsersAsync();
        Task<List<WorkoutUserDto>> GetByUserIdAsync(int userId);
        Task<WorkoutUserDto> CreateWorkoutUserAsync(WorkoutUserDto dto);
        Task<WorkoutUserDto> UpdateWorkoutUserAsync(WorkoutUserDto dto);
        Task DeleteWorkoutUserAsync(int id);
    }
}

