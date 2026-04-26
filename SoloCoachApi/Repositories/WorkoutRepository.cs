using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly ApplicationContext _context;
        private readonly WorkoutMapper _workoutMapper;

        public WorkoutRepository(ApplicationContext context, WorkoutMapper workoutMapper)
        {
            _context = context;
            _workoutMapper = workoutMapper;
        }

        public async Task<WorkoutDto> GetWorkoutByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID тренировки должен быть положительным числом", nameof(id));
            }

            var workout = await _context.Workouts.FindAsync(id);

            if (workout == null)
            {
                throw new KeyNotFoundException($"Тренировка с ID {id} не найдена");
            }

            var trainingExercises = await _context.TrainingExercises
                .AsNoTracking()
                .Where(te => te.WorkoutId == id)
                .Include(te => te.Exercise)
                .OrderBy(te => te.ExecutionOrder)
                .ToListAsync();

            return _workoutMapper.ToDto(workout, trainingExercises);
        }

        public async Task<List<WorkoutDto>> GetAllWorkoutsAsync()
        {
            var workouts = await _context.Workouts.ToListAsync();
            return workouts.Select(w => _workoutMapper.ToDto(w)).ToList();
        }

        public async Task<PagedResultDto<WorkoutDto>> GetWorkoutsPagedAsync(
            int page,
            int pageSize,
            string? search,
            string? typeWorkout,
            string? complexity)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var q = _context.Workouts.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                q = q.Where(w =>
                    EF.Functions.ILike(w.Name, $"%{term}%") ||
                    EF.Functions.ILike(w.Description, $"%{term}%"));
            }

            if (!string.IsNullOrWhiteSpace(typeWorkout))
            {
                var t = typeWorkout.Trim();
                q = q.Where(w => w.TypeWorkout == t);
            }

            if (!string.IsNullOrWhiteSpace(complexity))
            {
                var c = complexity.Trim();
                q = q.Where(w => w.Complexity == c);
            }

            var total = await q.CountAsync();
            var items = await q
                .OrderBy(w => w.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDto<WorkoutDto>
            {
                Items = items.Select(w => _workoutMapper.ToDto(w)).ToList(),
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<WorkoutDto> CreateWorkoutAsync(WorkoutDto dto)
        {
            var entity = _workoutMapper.ToModel(dto);
            entity.IdWorkout = 0;

            _context.Workouts.Add(entity);
            await _context.SaveChangesAsync();

            return _workoutMapper.ToDto(entity);
        }

        public async Task<WorkoutDto> UpdateWorkoutAsync(WorkoutDto dto)
        {
            if (dto.IdWorkout <= 0)
            {
                throw new ArgumentException("ID тренировки должен быть положительным числом", nameof(dto.IdWorkout));
            }

            var existing = await _context.Workouts.FindAsync(dto.IdWorkout);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Тренировка с ID {dto.IdWorkout} не найдена");
            }

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Duration = dto.Duration;
            existing.Complexity = dto.Complexity;
            existing.TypeWorkout = dto.TypeWorkout;

            await _context.SaveChangesAsync();

            return _workoutMapper.ToDto(existing);
        }

        public async Task DeleteWorkoutAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID тренировки должен быть положительным числом", nameof(id));
            }

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                throw new KeyNotFoundException($"Тренировка с ID {id} не найдена");
            }

            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
        }
    }
}