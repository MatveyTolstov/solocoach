using Microsoft.EntityFrameworkCore;
using SoloCoachApi.DataBase;
using SoloCoachApi.Mappers;
using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public class MetricsUserRepository : IMetricsUserRepository
    {
        private readonly ApplicationContext _context;
        private readonly MetricsUserMapper _metricsUserMapper;

        public MetricsUserRepository(ApplicationContext context, MetricsUserMapper metricsUserMapper)
        {
            _context = context;
            _metricsUserMapper = metricsUserMapper;
        }

        public async Task<MetricsUserDto> GetMetricsUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID метрик пользователя должен быть положительным числом", nameof(id));
            }

            var metrics = await _context.MetricsUsers.FindAsync(id);

            if (metrics == null)
            {
                throw new KeyNotFoundException($"Метрики пользователя с ID {id} не существуют");
            }

            return _metricsUserMapper.ToDto(metrics);
        }

        public async Task<List<MetricsUserDto>> GetAllMetricsUsersAsync()
        {
            var metrics = await _context.MetricsUsers.ToListAsync();
            return metrics.Select(_metricsUserMapper.ToDto).ToList();
        }

        public async Task<MetricsUserDto> CreateMetricsUserAsync(MetricsUserDto dto)
        {
            var entity = _metricsUserMapper.ToModel(dto);
            entity.IdMetricsUser = 0;

            _context.MetricsUsers.Add(entity);
            await _context.SaveChangesAsync();

            return _metricsUserMapper.ToDto(entity);
        }

        public async Task<MetricsUserDto> UpdateMetricsUserAsync(MetricsUserDto dto)
        {
            if (dto.IdMetricsUser <= 0)
            {
                throw new ArgumentException("ID метрик пользователя должен быть положительным числом", nameof(dto.IdMetricsUser));
            }

            var existing = await _context.MetricsUsers.FindAsync(dto.IdMetricsUser);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Метрики пользователя с ID {dto.IdMetricsUser} не существуют");
            }

            existing.Gender = dto.Gender;
            existing.Age = dto.Age;
            existing.Height = dto.Height;
            existing.Weight = dto.Weight;
            existing.ExperienceLevel = dto.ExperienceLevel;
            existing.ActivityLevel = dto.ActivityLevel;

            if (!dto.GoalId.HasValue || dto.GoalId.Value <= 0)
            {
                throw new ArgumentException("GoalId должен быть указан и больше 0", nameof(dto.GoalId));
            }

            existing.GoalId = dto.GoalId.Value;

            await _context.SaveChangesAsync();

            return _metricsUserMapper.ToDto(existing);
        }

        public async Task DeleteMetricsUserAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID метрик пользователя должен быть положительным числом", nameof(id));
            }

            var metrics = await _context.MetricsUsers.FindAsync(id);
            if (metrics == null)
            {
                throw new KeyNotFoundException($"Метрики пользователя с ID {id} не существуют");
            }

            var linkedUsers = await _context.Users
                .Where(u => u.MetricsUserId == id)
                .ToListAsync();
            foreach (var u in linkedUsers)
                u.MetricsUserId = null;

            _context.MetricsUsers.Remove(metrics);
            await _context.SaveChangesAsync();
        }
    }
}