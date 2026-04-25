using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class WorkoutUserLogRepository : IWorkoutUserLogRepository
    {
        private readonly ApplicationContext _context;
        private readonly WorkoutUserLogMapper _workoutUserLogMapper;

        public WorkoutUserLogRepository(ApplicationContext context, WorkoutUserLogMapper workoutUserLogMapper)
        {
            _context = context;
            _workoutUserLogMapper = workoutUserLogMapper;
        }

        public async Task<WorkoutUserLogDto> GetWorkoutUserLogByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID workout user log must be a positive number", nameof(id));
            }

            var log = await _context.WorkoutUserLogs.FindAsync(id);

            if (log == null)
            {
                throw new KeyNotFoundException($"WorkoutUserLog with ID {id} does not exist");
            }

            return _workoutUserLogMapper.ToDto(log);
        }

        public async Task<List<WorkoutUserLogDto>> GetAllWorkoutUserLogsAsync()
        {
            var logs = await _context.WorkoutUserLogs.ToListAsync();
            return logs.Select(_workoutUserLogMapper.ToDto).ToList();
        }

        public async Task<List<WorkoutUserLogDto>> GetByWorkoutUserIdAsync(int workoutUserId)
        {
            var logs = await _context.WorkoutUserLogs
                .AsNoTracking()
                .Where(l => l.WorkoutUserId == workoutUserId)
                .OrderBy(l => l.IdWorkoutUserLog)
                .ToListAsync();
            return logs.Select(_workoutUserLogMapper.ToDto).ToList();
        }

        public async Task<WorkoutUserLogDto> CreateWorkoutUserLogAsync(WorkoutUserLogDto dto)
        {
            var entity = _workoutUserLogMapper.ToModel(dto);
            entity.IdWorkoutUserLog = 0;

            _context.WorkoutUserLogs.Add(entity);
            await _context.SaveChangesAsync();

            return _workoutUserLogMapper.ToDto(entity);
        }

        public async Task<WorkoutUserLogDto> UpdateWorkoutUserLogAsync(WorkoutUserLogDto dto)
        {
            if (dto.IdWorkoutUserLog <= 0)
            {
                throw new ArgumentException("ID workout user log must be a positive number", nameof(dto.IdWorkoutUserLog));
            }

            var existing = await _context.WorkoutUserLogs.FindAsync(dto.IdWorkoutUserLog);
            if (existing == null)
            {
                throw new KeyNotFoundException($"WorkoutUserLog with ID {dto.IdWorkoutUserLog} does not exist");
            }

            existing.IdWorkoutUserLog = dto.IdWorkoutUserLog;
            existing.WorkoutId = dto.WorkoutId;
            existing.WorkoutUserId = dto.WorkoutUserId;
            existing.Repetitions = dto.Repetitions;
            existing.Sets = dto.Sets;
            existing.Weight = dto.Weight;
            existing.Status = dto.Status;

            await _context.SaveChangesAsync();

            return _workoutUserLogMapper.ToDto(existing);
        }

        public async Task DeleteWorkoutUserLogAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID workout user log must be a positive number", nameof(id));
            }

            var log = await _context.WorkoutUserLogs.FindAsync(id);
            if (log == null)
            {
                throw new KeyNotFoundException($"WorkoutUserLog with ID {id} does not exist");
            }

            _context.WorkoutUserLogs.Remove(log);
            await _context.SaveChangesAsync();
        }
    }
}

