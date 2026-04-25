using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class ExerciseGroupsMuscleRepository : IExerciseGroupsMuscleRepository
    {
        private readonly ApplicationContext _context;
        private readonly ExerciseGroupsMuscleMapper _exerciseGroupsMuscleMapper;

        public ExerciseGroupsMuscleRepository(ApplicationContext context, ExerciseGroupsMuscleMapper exerciseGroupsMuscleMapper)
        {
            _context = context;
            _exerciseGroupsMuscleMapper = exerciseGroupsMuscleMapper;
        }

        public async Task<ExerciseGroupsMuscleDto> GetExerciseGroupsMuscleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID exercise-groups-muscle must be a positive number", nameof(id));
            }

            var entity = await _context.ExercisesGroups.FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"ExerciseGroupsMuscle with ID {id} does not exist");
            }

            return _exerciseGroupsMuscleMapper.ToDto(entity);
        }

        public async Task<List<ExerciseGroupsMuscleDto>> GetAllExerciseGroupsMusclesAsync()
        {
            var entities = await _context.ExercisesGroups.ToListAsync();
            return entities.Select(_exerciseGroupsMuscleMapper.ToDto).ToList();
        }

        public async Task<ExerciseGroupsMuscleDto> CreateExerciseGroupsMuscleAsync(ExerciseGroupsMuscleDto dto)
        {
            var entity = _exerciseGroupsMuscleMapper.ToModel(dto);
            entity.IdExerciseGroupsMuscle = 0;

            _context.ExercisesGroups.Add(entity);
            await _context.SaveChangesAsync();

            return _exerciseGroupsMuscleMapper.ToDto(entity);
        }

        public async Task<ExerciseGroupsMuscleDto> UpdateExerciseGroupsMuscleAsync(ExerciseGroupsMuscleDto dto)
        {
            if (dto.IdExerciseGroupsMuscle <= 0)
            {
                throw new ArgumentException("ID exercise-groups-muscle must be a positive number", nameof(dto.IdExerciseGroupsMuscle));
            }

            var existing = await _context.ExercisesGroups.FindAsync(dto.IdExerciseGroupsMuscle);
            if (existing == null)
            {
                throw new KeyNotFoundException($"ExerciseGroupsMuscle with ID {dto.IdExerciseGroupsMuscle} does not exist");
            }

            existing.ExerciseId = dto.ExerciseId;
            existing.GroupsMusclesId = dto.GroupsMusclesId;

            await _context.SaveChangesAsync();

            return _exerciseGroupsMuscleMapper.ToDto(existing);
        }

        public async Task DeleteExerciseGroupsMuscleAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID exercise-groups-muscle must be a positive number", nameof(id));
            }

            var entity = await _context.ExercisesGroups.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"ExerciseGroupsMuscle with ID {id} does not exist");
            }

            _context.ExercisesGroups.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}

