using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class PlanWorkoutRepository : IPlanWorkoutRepository
    {
        private readonly ApplicationContext _context;
        private readonly PlanWorkoutMapper _planWorkoutMapper;

        public PlanWorkoutRepository(ApplicationContext context, PlanWorkoutMapper planWorkoutMapper)
        {
            _context = context;
            _planWorkoutMapper = planWorkoutMapper;
        }

        public async Task<PlanWorkoutDto> GetPlanWorkoutByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID плана тренировки должен быть положительным числом", nameof(id));
            }

            var plan = await _context.PlanWorkouts.FindAsync(id);

            if (plan == null)
            {
                throw new KeyNotFoundException($"План тренировки с ID {id} не существует");
            }

            return _planWorkoutMapper.ToDto(plan);
        }

        public async Task<List<PlanWorkoutDto>> GetAllPlanWorkoutsAsync()
        {
            var plans = await _context.PlanWorkouts.ToListAsync();
            return plans.Select(_planWorkoutMapper.ToDto).ToList();
        }

        public async Task<PlanWorkoutDto> CreatePlanWorkoutAsync(PlanWorkoutDto dto)
        {
            var entity = _planWorkoutMapper.ToModel(dto);
            entity.IdPlanWorkout = 0;

            _context.PlanWorkouts.Add(entity);
            await _context.SaveChangesAsync();

            return _planWorkoutMapper.ToDto(entity);
        }

        public async Task<PlanWorkoutDto> UpdatePlanWorkoutAsync(PlanWorkoutDto dto)
        {
            if (dto.IdPlanWorkout <= 0)
            {
                throw new ArgumentException("ID плана тренировки должен быть положительным числом", nameof(dto.IdPlanWorkout));
            }

            var existing = await _context.PlanWorkouts.FindAsync(dto.IdPlanWorkout);
            if (existing == null)
            {
                throw new KeyNotFoundException($"План тренировки с ID {dto.IdPlanWorkout} не существует");
            }

            existing.WorkoutId = dto.WorkoutId;
            existing.UserId = dto.UserId;

            await _context.SaveChangesAsync();

            return _planWorkoutMapper.ToDto(existing);
        }

        public async Task DeletePlanWorkoutAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID плана тренировки должен быть положительным числом", nameof(id));
            }

            var plan = await _context.PlanWorkouts.FindAsync(id);
            if (plan == null)
            {
                throw new KeyNotFoundException($"План тренировки с ID {id} не существует");
            }

            _context.PlanWorkouts.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }
}