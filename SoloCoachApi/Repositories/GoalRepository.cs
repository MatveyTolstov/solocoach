using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly ApplicationContext _context;
        private readonly GoalMapper _goalMapper;

        public GoalRepository(ApplicationContext context, GoalMapper goalMapper)
        {
            _context = context;
            _goalMapper = goalMapper;
        }

        public async Task<GoalDto> GetGoalByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID goal must be a positive number", nameof(id));
            }

            var goal = await _context.Goals.FindAsync(id);

            if (goal == null)
            {
                throw new KeyNotFoundException($"Goal with ID {id} does not exist");
            }

            return _goalMapper.ToDto(goal);
        }

        public async Task<List<GoalDto>> GetAllGoalsAsync()
        {
            var goals = await _context.Goals.ToListAsync();
            return goals.Select(_goalMapper.ToDto).ToList();
        }

        public async Task<GoalDto> CreateGoalAsync(GoalDto dto)
        {
            var entity = _goalMapper.ToModel(dto);
            entity.IdGoal = 0;

            _context.Goals.Add(entity);
            await _context.SaveChangesAsync();

            return _goalMapper.ToDto(entity);
        }

        public async Task<GoalDto> UpdateGoalAsync(GoalDto dto)
        {
            if (dto.IdGoal <= 0)
            {
                throw new ArgumentException("ID goal must be a positive number", nameof(dto.IdGoal));
            }

            var existing = await _context.Goals.FindAsync(dto.IdGoal);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Goal with ID {dto.IdGoal} does not exist");
            }

            existing.DateEnd = dto.DateEnd;
            existing.DateStart = dto.DateStart;
            existing.Status = dto.Status;
            existing.TargetWeight = dto.TargetWeight;
            existing.TypeGoal = dto.TypeGoal;

            await _context.SaveChangesAsync();

            return _goalMapper.ToDto(existing);
        }

        public async Task DeleteGoalAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID goal must be a positive number", nameof(id));
            }

            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                throw new KeyNotFoundException($"Goal with ID {id} does not exist");
            }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
        }
    }
}

