using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class TrainingExerciseService
    {
        private readonly ITrainingExerciseRepository _trainingExerciseRepository;

        public TrainingExerciseService(ITrainingExerciseRepository trainingExerciseRepository)
        {
            _trainingExerciseRepository = trainingExerciseRepository;
        }

        public Task<TrainingExerciseDto> GetByIdAsync(int id) => _trainingExerciseRepository.GetTrainingExerciseByIdAsync(id);

        public Task<List<TrainingExerciseDto>> GetAllAsync() => _trainingExerciseRepository.GetAllTrainingExercisesAsync();

        public Task<TrainingExerciseDto> CreateAsync(TrainingExerciseDto dto) => _trainingExerciseRepository.CreateTrainingExerciseAsync(dto);

        public Task<TrainingExerciseDto> UpdateAsync(TrainingExerciseDto dto) => _trainingExerciseRepository.UpdateTrainingExerciseAsync(dto);

        public Task DeleteAsync(int id) => _trainingExerciseRepository.DeleteTrainingExerciseAsync(id);
    }
}

