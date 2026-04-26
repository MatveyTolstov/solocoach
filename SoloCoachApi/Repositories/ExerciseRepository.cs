using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class ExerciseRepository : IExerciseRepository
    {
        private readonly ApplicationContext _context;
        private readonly ExerciseMapper _exerciseMapper;

        public ExerciseRepository(ApplicationContext context, ExerciseMapper exerciseMapper)
        {
            _context = context;
            _exerciseMapper = exerciseMapper;
        }

        public async Task<ExerciseDto> GetExerciseByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID упражнения должен быть положительным числом", nameof(id));
            }

            var exercise = await _context.Exercises.FindAsync(id);

            if (exercise == null)
            {
                throw new KeyNotFoundException($"Упражнение с ID {id} не существует");
            }

            return _exerciseMapper.ToDto(exercise);
        }

        public async Task<List<ExerciseDto>> GetAllExercisesAsync()
        {
            var exercises = await _context.Exercises.ToListAsync();
            return exercises.Select(_exerciseMapper.ToDto).ToList();
        }

        public async Task<PagedResultDto<ExerciseDto>> GetExercisesPagedAsync(
            int page,
            int pageSize,
            string? search,
            string? complexity)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _context.Exercises.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(e =>
                    EF.Functions.ILike(e.Name, $"%{term}%") ||
                    (e.Description != null && EF.Functions.ILike(e.Description, $"%{term}%")));
            }

            if (!string.IsNullOrWhiteSpace(complexity))
            {
                var c = complexity.Trim();
                q = q.Where(e => e.Complexity != null && e.Complexity == c);
            }

            var total = await q.CountAsync();
            var items = await q
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<ExerciseDto>
            {
                Items = items.Select(_exerciseMapper.ToDto).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ExerciseDto> CreateExerciseAsync(ExerciseDto dto)
        {
            var entity = _exerciseMapper.ToModel(dto);
            entity.IdExercise = 0;

            _context.Exercises.Add(entity);
            await _context.SaveChangesAsync();

            return _exerciseMapper.ToDto(entity);
        }

        public async Task<ExerciseDto> UpdateExerciseAsync(ExerciseDto dto)
        {
            if (dto.IdExercise <= 0)
            {
                throw new ArgumentException("ID упражнения должен быть положительным числом", nameof(dto.IdExercise));
            }

            var existing = await _context.Exercises.FindAsync(dto.IdExercise);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Упражнение с ID {dto.IdExercise} не существует");
            }

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.PictureUrl = dto.PictureUrl;
            existing.Complexity = dto.Complexity;

            await _context.SaveChangesAsync();

            return _exerciseMapper.ToDto(existing);
        }

        public async Task DeleteExerciseAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID упражнения должен быть положительным числом", nameof(id));
            }

            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null)
            {
                throw new KeyNotFoundException($"Упражнение с ID {id} не существует");
            }

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
        }
    }
}