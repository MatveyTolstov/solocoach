using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class WorkoutService
    {
        private readonly IWorkoutRepository _workoutRepository;

        public WorkoutService(IWorkoutRepository workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public Task<WorkoutDto> GetByIdAsync(int id) => _workoutRepository.GetWorkoutByIdAsync(id);

        public Task<List<WorkoutDto>> GetAllAsync() => _workoutRepository.GetAllWorkoutsAsync();

        public Task<PagedResultDto<WorkoutDto>> GetPagedAsync(WorkoutListQuery query) =>
            _workoutRepository.GetWorkoutsPagedAsync(
                query.Page,
                query.PageSize,
                query.Search,
                query.TypeWorkout,
                query.Complexity);

        public Task<WorkoutDto> CreateAsync(WorkoutDto dto) => _workoutRepository.CreateWorkoutAsync(dto);

        public Task<WorkoutDto> UpdateAsync(WorkoutDto dto) => _workoutRepository.UpdateWorkoutAsync(dto);

        public Task DeleteAsync(int id) => _workoutRepository.DeleteWorkoutAsync(id);
    }
}

