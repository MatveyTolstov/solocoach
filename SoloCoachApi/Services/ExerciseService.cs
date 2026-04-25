using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class ExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;

        public ExerciseService(IExerciseRepository exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }

        public Task<ExerciseDto> GetByIdAsync(int id) => _exerciseRepository.GetExerciseByIdAsync(id);

        public Task<List<ExerciseDto>> GetAllAsync() => _exerciseRepository.GetAllExercisesAsync();

        public Task<PagedResultDto<ExerciseDto>> GetPagedAsync(ExerciseListQuery query) =>
            _exerciseRepository.GetExercisesPagedAsync(
                query.Page,
                query.PageSize,
                query.Search,
                query.Complexity);

        public Task<ExerciseDto> CreateAsync(ExerciseDto dto) => _exerciseRepository.CreateExerciseAsync(dto);

        public Task<ExerciseDto> UpdateAsync(ExerciseDto dto) => _exerciseRepository.UpdateExerciseAsync(dto);

        public Task DeleteAsync(int id) => _exerciseRepository.DeleteExerciseAsync(id);
    }
}

