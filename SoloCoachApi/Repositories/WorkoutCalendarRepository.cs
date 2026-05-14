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
                throw new ArgumentException("ID календаря тренировок должен быть положительным числом", nameof(id));
            }

            var calendar = await _context.WorkoutCalendars.FindAsync(id);

            if (calendar == null)
            {
                throw new KeyNotFoundException($"Календарь тренировок с ID {id} не найден");
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
                throw new ArgumentException("ID календаря тренировок должен быть положительным числом", nameof(dto.IdWorkoutCalendar));
            }

            var existing = await _context.WorkoutCalendars.FindAsync(dto.IdWorkoutCalendar);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Календарь тренировок с ID {dto.IdWorkoutCalendar} не найден");
            }

            existing.UserId = dto.UserId;
            existing.WorkoutId = dto.WorkoutId;
            existing.Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
            existing.Status = dto.Status;

            await _context.SaveChangesAsync();

            return _workoutCalendarMapper.ToDto(existing);
        }

        public async Task DeleteWorkoutCalendarAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID календаря тренировок должен быть положительным числом", nameof(id));
            }

            var calendar = await _context.WorkoutCalendars.FindAsync(id);
            if (calendar == null)
            {
                throw new KeyNotFoundException($"Календарь тренировок с ID {id} не найден");
            }

            _context.WorkoutCalendars.Remove(calendar);
            await _context.SaveChangesAsync();
        }
    }
}