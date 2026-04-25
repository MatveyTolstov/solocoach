using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface ITrainingExerciseRepository
    {
        Task<TrainingExerciseDto> GetTrainingExerciseByIdAsync(int id);
        Task<List<TrainingExerciseDto>> GetAllTrainingExercisesAsync();
        Task<TrainingExerciseDto> CreateTrainingExerciseAsync(TrainingExerciseDto dto);
        Task<TrainingExerciseDto> UpdateTrainingExerciseAsync(TrainingExerciseDto dto);
        Task DeleteTrainingExerciseAsync(int id);
    }
}

