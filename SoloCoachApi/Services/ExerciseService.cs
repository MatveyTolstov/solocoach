using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class ExerciseService
    {
        private readonly IExerciseRepository _exerciseRepository;
        private readonly S3Service _s3Service;

        public ExerciseService(IExerciseRepository exerciseRepository, S3Service s3Service)
        {
            _exerciseRepository = exerciseRepository;
            _s3Service = s3Service;
        }

        public Task<ExerciseDto> GetByIdAsync(int id) => _exerciseRepository.GetExerciseByIdAsync(id);

        public Task<List<ExerciseDto>> GetAllAsync() => _exerciseRepository.GetAllExercisesAsync();

        public Task<PagedResultDto<ExerciseDto>> GetPagedAsync(ExerciseListQuery query) =>
            _exerciseRepository.GetExercisesPagedAsync(
                query.Page,
                query.PageSize,
                query.Search,
                query.Complexity,
                query.MuscleGroup);

        public Task<ExerciseDto> CreateAsync(ExerciseDto dto) => _exerciseRepository.CreateExerciseAsync(dto);

        public Task<ExerciseDto> UpdateAsync(ExerciseDto dto) => _exerciseRepository.UpdateExerciseAsync(dto);

        public Task DeleteAsync(int id) => _exerciseRepository.DeleteExerciseAsync(id);

        public async Task<ExerciseDto> UploadVideoAsync(int id, IFormFile file)
        {
            var existing = await _exerciseRepository.GetExerciseByIdAsync(id);
            if (existing.VideoUrl != null)
            {
                var oldKey = _s3Service.ExtractKey(existing.VideoUrl);
                await _s3Service.DeleteAsync(oldKey);
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var key = $"exercises/{id}/{Guid.NewGuid()}{ext}";

            using var stream = file.OpenReadStream();
            var url = await _s3Service.UploadAsync(key, stream, file.ContentType);

            return await _exerciseRepository.UpdateVideoUrlAsync(id, url);
        }

        public async Task<ExerciseDto> DeleteVideoAsync(int id)
        {
            var exercise = await _exerciseRepository.GetExerciseByIdAsync(id);

            if (exercise.VideoUrl != null)
            {
                var key = _s3Service.ExtractKey(exercise.VideoUrl);
                await _s3Service.DeleteAsync(key);
            }

            return await _exerciseRepository.UpdateVideoUrlAsync(id, null);
        }
    }
}

