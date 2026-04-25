using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class WorkoutUserRepository : IWorkoutUserRepository
    {
        private readonly ApplicationContext _context;
        private readonly WorkoutUserMapper _workoutUserMapper;

        public WorkoutUserRepository(ApplicationContext context, WorkoutUserMapper workoutUserMapper)
        {
            _context = context;
            _workoutUserMapper = workoutUserMapper;
        }

        public async Task<WorkoutUserDto> GetWorkoutUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID workout user must be a positive number", nameof(id));
            }

            var workoutUser = await _context.WorkoutUser.FindAsync(id);

            if (workoutUser == null)
            {
                throw new KeyNotFoundException($"WorkoutUser with ID {id} does not exist");
            }

            return _workoutUserMapper.ToDto(workoutUser);
        }

        public async Task<List<WorkoutUserDto>> GetAllWorkoutUsersAsync()
        {
            var workoutsUsers = await _context.WorkoutUser.ToListAsync();
            return workoutsUsers.Select(_workoutUserMapper.ToDto).ToList();
        }

        public async Task<List<WorkoutUserDto>> GetByUserIdAsync(int userId)
        {
            var list = await _context.WorkoutUser
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .ToListAsync();
            return list.Select(_workoutUserMapper.ToDto).ToList();
        }

        public async Task<WorkoutUserDto> CreateWorkoutUserAsync(WorkoutUserDto dto)
        {
            var entity = _workoutUserMapper.ToModel(dto);
            entity.IdWorkoutUser = 0;

            _context.WorkoutUser.Add(entity);
            await _context.SaveChangesAsync();

            return _workoutUserMapper.ToDto(entity);
        }

        public async Task<WorkoutUserDto> UpdateWorkoutUserAsync(WorkoutUserDto dto)
        {
            if (dto.IdWorkoutUser <= 0)
            {
                throw new ArgumentException("ID workout user must be a positive number", nameof(dto.IdWorkoutUser));
            }

            var existing = await _context.WorkoutUser.FindAsync(dto.IdWorkoutUser);
            if (existing == null)
            {
                throw new KeyNotFoundException($"WorkoutUser with ID {dto.IdWorkoutUser} does not exist");
            }

            existing.IdWorkoutUser = dto.IdWorkoutUser;
            existing.WorkoutId = dto.WorkoutId;
            existing.Date = dto.Date;
            existing.Duration = dto.Duration;
            existing.Status = dto.Status;

            await _context.SaveChangesAsync();

            return _workoutUserMapper.ToDto(existing);
        }

        public async Task DeleteWorkoutUserAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID workout user must be a positive number", nameof(id));
            }

            var workoutUser = await _context.WorkoutUser.FindAsync(id);
            if (workoutUser == null)
            {
                throw new KeyNotFoundException($"WorkoutUser with ID {id} does not exist");
            }

            _context.WorkoutUser.Remove(workoutUser);
            await _context.SaveChangesAsync();
        }
    }
}

