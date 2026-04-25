using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class ExerciseGroupsMuscleService
    {
        private readonly IExerciseGroupsMuscleRepository _exerciseGroupsMuscleRepository;

        public ExerciseGroupsMuscleService(IExerciseGroupsMuscleRepository exerciseGroupsMuscleRepository)
        {
            _exerciseGroupsMuscleRepository = exerciseGroupsMuscleRepository;
        }

        public Task<ExerciseGroupsMuscleDto> GetByIdAsync(int id) => _exerciseGroupsMuscleRepository.GetExerciseGroupsMuscleByIdAsync(id);

        public Task<List<ExerciseGroupsMuscleDto>> GetAllAsync() => _exerciseGroupsMuscleRepository.GetAllExerciseGroupsMusclesAsync();

        public Task<ExerciseGroupsMuscleDto> CreateAsync(ExerciseGroupsMuscleDto dto) => _exerciseGroupsMuscleRepository.CreateExerciseGroupsMuscleAsync(dto);

        public Task<ExerciseGroupsMuscleDto> UpdateAsync(ExerciseGroupsMuscleDto dto) => _exerciseGroupsMuscleRepository.UpdateExerciseGroupsMuscleAsync(dto);

        public Task DeleteAsync(int id) => _exerciseGroupsMuscleRepository.DeleteExerciseGroupsMuscleAsync(id);
    }
}

