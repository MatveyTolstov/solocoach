using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Services;

public class WorkoutSessionService
{
    private readonly ApplicationContext _context;
    private readonly WorkoutUserMapper _workoutUserMapper;
    private readonly WorkoutUserLogMapper _workoutUserLogMapper;

    public WorkoutSessionService(
        ApplicationContext context,
        WorkoutUserMapper workoutUserMapper,
        WorkoutUserLogMapper workoutUserLogMapper)
    {
        _context = context;
        _workoutUserMapper = workoutUserMapper;
        _workoutUserLogMapper = workoutUserLogMapper;
    }

    public async Task<CompleteWorkoutSessionResultDto> CompleteAsync(int userId, CompleteWorkoutSessionDto dto)
    {
        if (dto.Logs.Count == 0)
        {
            throw new ArgumentException("Добавьте хотя бы одну строку журнала (подходы).");
        }

        var workout = await _context.Workouts.FindAsync(dto.WorkoutId);
        if (workout == null)
        {
            throw new KeyNotFoundException($"Тренировка с ID {dto.WorkoutId} не найдена.");
        }

        await using var tx = await _context.Database.BeginTransactionAsync();
        WorkoutUser wu;
        try
        {
            wu = new WorkoutUser
            {
                UserId = userId,
                WorkoutId = dto.WorkoutId,
                Duration = dto.Duration,
                Date = dto.Date,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Завершена" : dto.Status
            };

            _context.WorkoutUser.Add(wu);
            await _context.SaveChangesAsync();

            foreach (var line in dto.Logs)
            {
                _context.WorkoutUserLogs.Add(new WorkoutUserLog
                {
                    WorkoutUserId = wu.IdWorkoutUser,
                    WorkoutId = dto.WorkoutId,
                    Repetitions = line.Repetitions,
                    Sets = line.Sets,
                    Weight = line.Weight,
                    Status = line.Status
                });
            }

            await _context.SaveChangesAsync();

            if (dto.CalendarEntryId is int cid && cid > 0)
            {
                var cal = await _context.WorkoutCalendars.FindAsync(cid);
                if (cal != null && cal.UserId == userId)
                {
                    cal.Status = global::SoloCoachApi.WorkoutCalendarStatus.Completed;
                }
            }
            else
            {
                await UpsertCalendarCompletedAsync(userId, dto.WorkoutId, dto.Date);
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        var workoutUserId = wu.IdWorkoutUser;
        var logs = await _context.WorkoutUserLogs
            .AsNoTracking()
            .Where(l => l.WorkoutUserId == workoutUserId)
            .OrderBy(l => l.IdWorkoutUserLog)
            .ToListAsync();

        return new CompleteWorkoutSessionResultDto
        {
            WorkoutUser = _workoutUserMapper.ToDto(wu),
            Logs = logs.Select(_workoutUserLogMapper.ToDto).ToList()
        };
    }

    /// <summary>
    /// Если тренировка завершена не из календаря — всё равно отражаем её в календаре на дату сессии (выполнена).
    /// Если на эту дату уже была запланирована та же тренировка — помечаем выполненной.
    /// </summary>
    private async Task UpsertCalendarCompletedAsync(int userId, int workoutId, DateTime sessionDate)
    {
        var day = sessionDate.Kind == DateTimeKind.Utc
            ? sessionDate
            : sessionDate.ToUniversalTime();

        var candidates = await _context.WorkoutCalendars
            .Where(c => c.UserId == userId && c.WorkoutId == workoutId)
            .ToListAsync();

        WorkoutCalendar? match = null;
        foreach (var c in candidates)
        {
            var cDay = c.Date.Kind == DateTimeKind.Utc ? c.Date : c.Date.ToUniversalTime();
            if (cDay.Year == day.Year && cDay.Month == day.Month && cDay.Day == day.Day)
            {
                match = c;
                break;
            }
        }

        if (match != null)
        {
            match.Status = global::SoloCoachApi.WorkoutCalendarStatus.Completed;
        }
        else
        {
            _context.WorkoutCalendars.Add(new WorkoutCalendar
            {
                UserId = userId,
                WorkoutId = workoutId,
                Date = day,
                Status = global::SoloCoachApi.WorkoutCalendarStatus.Completed
            });
        }
    }
}
