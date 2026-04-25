using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class WorkoutUserLogService
    {
        private readonly IWorkoutUserLogRepository _workoutUserLogRepository;
        private readonly IWorkoutUserRepository _workoutUserRepository;

        public WorkoutUserLogService(
            IWorkoutUserLogRepository workoutUserLogRepository,
            IWorkoutUserRepository workoutUserRepository)
        {
            _workoutUserLogRepository = workoutUserLogRepository;
            _workoutUserRepository = workoutUserRepository;
        }

        public Task<WorkoutUserLogDto> GetByIdAsync(int id) => _workoutUserLogRepository.GetWorkoutUserLogByIdAsync(id);

        public Task<List<WorkoutUserLogDto>> GetAllAsync() => _workoutUserLogRepository.GetAllWorkoutUserLogsAsync();

        public async Task<List<WorkoutUserLogDto>> GetByWorkoutUserForUserAsync(int workoutUserId, int userId)
        {
            var wu = await _workoutUserRepository.GetWorkoutUserByIdAsync(workoutUserId);
            if (wu.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            return await _workoutUserLogRepository.GetByWorkoutUserIdAsync(workoutUserId);
        }

        public Task<WorkoutUserLogDto> CreateAsync(WorkoutUserLogDto dto) => _workoutUserLogRepository.CreateWorkoutUserLogAsync(dto);

        public Task<WorkoutUserLogDto> UpdateAsync(WorkoutUserLogDto dto) => _workoutUserLogRepository.UpdateWorkoutUserLogAsync(dto);

        public Task DeleteAsync(int id) => _workoutUserLogRepository.DeleteWorkoutUserLogAsync(id);
    }
}

