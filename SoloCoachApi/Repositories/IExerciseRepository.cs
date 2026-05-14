using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IExerciseRepository
    {
        Task<ExerciseDto> GetExerciseByIdAsync(int id);
        Task<List<ExerciseDto>> GetAllExercisesAsync();
        Task<PagedResultDto<ExerciseDto>> GetExercisesPagedAsync(int page, int pageSize, string? search, string? complexity, string? muscleGroup);
        Task<ExerciseDto> CreateExerciseAsync(ExerciseDto dto);
        Task<ExerciseDto> UpdateExerciseAsync(ExerciseDto dto);
        Task DeleteExerciseAsync(int id);
        Task<ExerciseDto> UpdateVideoUrlAsync(int id, string? videoUrl);
    }
}

