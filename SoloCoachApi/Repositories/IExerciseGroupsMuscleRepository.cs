using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IExerciseGroupsMuscleRepository
    {
        Task<ExerciseGroupsMuscleDto> GetExerciseGroupsMuscleByIdAsync(int id);
        Task<List<ExerciseGroupsMuscleDto>> GetAllExerciseGroupsMusclesAsync();
        Task<ExerciseGroupsMuscleDto> CreateExerciseGroupsMuscleAsync(ExerciseGroupsMuscleDto dto);
        Task<ExerciseGroupsMuscleDto> UpdateExerciseGroupsMuscleAsync(ExerciseGroupsMuscleDto dto);
        Task DeleteExerciseGroupsMuscleAsync(int id);
    }
}

