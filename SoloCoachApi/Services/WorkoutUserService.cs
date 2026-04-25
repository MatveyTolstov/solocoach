using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class WorkoutUserService
    {
        private readonly IWorkoutUserRepository _workoutUserRepository;

        public WorkoutUserService(IWorkoutUserRepository workoutUserRepository)
        {
            _workoutUserRepository = workoutUserRepository;
        }

        public Task<WorkoutUserDto> GetByIdAsync(int id) => _workoutUserRepository.GetWorkoutUserByIdAsync(id);

        public Task<List<WorkoutUserDto>> GetAllAsync() => _workoutUserRepository.GetAllWorkoutUsersAsync();

        public Task<List<WorkoutUserDto>> GetByUserIdAsync(int userId) => _workoutUserRepository.GetByUserIdAsync(userId);

        public Task<WorkoutUserDto> CreateAsync(WorkoutUserDto dto) => _workoutUserRepository.CreateWorkoutUserAsync(dto);

        public Task<WorkoutUserDto> UpdateAsync(WorkoutUserDto dto) => _workoutUserRepository.UpdateWorkoutUserAsync(dto);

        public Task DeleteAsync(int id) => _workoutUserRepository.DeleteWorkoutUserAsync(id);
    }
}

