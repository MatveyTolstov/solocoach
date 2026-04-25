using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class WorkoutCalendarRepository : IWorkoutCalendarRepository
    {
        private readonly ApplicationContext _context;
        private readonly WorkoutCalendarMapper _workoutCalendarMapper;

        public WorkoutCalendarRepository(ApplicationContext context, WorkoutCalendarMapper workoutCalendarMapper)
        {
            _context = context;
            _workoutCalendarMapper = workoutCalendarMapper;
        }

        public async Task<WorkoutCalendarDto> GetWorkoutCalendarByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID workout calendar must be a positive number", nameof(id));
            }

            var calendar = await _context.WorkoutCalendars.FindAsync(id);

            if (calendar == null)
            {
                throw new KeyNotFoundException($"WorkoutCalendar with ID {id} does not exist");
            }

            return _workoutCalendarMapper.ToDto(calendar);
        }

        public async Task<List<WorkoutCalendarDto>> GetAllWorkoutCalendarsAsync()
        {
            var calendars = await _context.WorkoutCalendars.ToListAsync();
            return calendars.Select(_workoutCalendarMapper.ToDto).ToList();
        }

        public async Task<List<WorkoutCalendarDto>> GetByUserIdAsync(int userId, DateTime? fromUtc, DateTime? toUtc)
        {
            var q = _context.WorkoutCalendars.AsNoTracking().Where(c => c.UserId == userId);
            if (fromUtc.HasValue)
            {
                q = q.Where(c => c.Date >= fromUtc.Value);
            }

            if (toUtc.HasValue)
            {
                q = q.Where(c => c.Date <= toUtc.Value);
            }

            var calendars = await q.OrderBy(c => c.Date).ToListAsync();
            return calendars.Select(_workoutCalendarMapper.ToDto).ToList();
        }

        public async Task<WorkoutCalendarDto> CreateWorkoutCalendarAsync(WorkoutCalendarDto dto)
        {
            var entity = _workoutCalendarMapper.ToModel(dto);
            entity.IdWorkoutCalendar = 0;

            _context.WorkoutCalendars.Add(entity);
            await _context.SaveChangesAsync();

            return _workoutCalendarMapper.ToDto(entity);
        }

        public async Task<WorkoutCalendarDto> UpdateWorkoutCalendarAsync(WorkoutCalendarDto dto)
        {
            if (dto.IdWorkoutCalendar <= 0)
            {
                throw new ArgumentException("ID workout calendar must be a positive number", nameof(dto.IdWorkoutCalendar));
            }

            var existing = await _context.WorkoutCalendars.FindAsync(dto.IdWorkoutCalendar);
            if (existing == null)
            {
                throw new KeyNotFoundException($"WorkoutCalendar with ID {dto.IdWorkoutCalendar} does not exist");
            }

            existing.IdWorkoutCalendar = dto.IdWorkoutCalendar;
            existing.UserId = dto.UserId;
            existing.WorkoutId = dto.WorkoutId;
            existing.Date = dto.Date;
            existing.Status = dto.Status;

            await _context.SaveChangesAsync();

            return _workoutCalendarMapper.ToDto(existing);
        }

        public async Task DeleteWorkoutCalendarAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID workout calendar must be a positive number", nameof(id));
            }

            var calendar = await _context.WorkoutCalendars.FindAsync(id);
            if (calendar == null)
            {
                throw new KeyNotFoundException($"WorkoutCalendar with ID {id} does not exist");
            }

            _context.WorkoutCalendars.Remove(calendar);
            await _context.SaveChangesAsync();
        }
    }
}

