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
                throw new ArgumentException("ID metrics user must be a positive number", nameof(id));
            }

            var metrics = await _context.MetricsUsers.FindAsync(id);

            if (metrics == null)
            {
                throw new KeyNotFoundException($"MetricsUser with ID {id} does not exist");
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
                throw new ArgumentException("ID metrics user must be a positive number", nameof(dto.IdMetricsUser));
            }

            var existing = await _context.MetricsUsers.FindAsync(dto.IdMetricsUser);
            if (existing == null)
            {
                throw new KeyNotFoundException($"MetricsUser with ID {dto.IdMetricsUser} does not exist");
            }

            existing.Gender = dto.Gender;
            existing.Age = dto.Age;
            existing.Height = dto.Height;
            existing.Weight = dto.Weight;
            existing.ExperienceLevel = dto.ExperienceLevel;
            existing.ActivityLevel = dto.ActivityLevel;
            if (!dto.GoalId.HasValue || dto.GoalId.Value <= 0)
            {
                throw new ArgumentException("GoalId must be provided and greater than 0", nameof(dto.GoalId));
            }
            existing.GoalId = dto.GoalId.Value;

            await _context.SaveChangesAsync();

            return _metricsUserMapper.ToDto(existing);
        }

        public async Task DeleteMetricsUserAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID metrics user must be a positive number", nameof(id));
            }

            var metrics = await _context.MetricsUsers.FindAsync(id);
            if (metrics == null)
            {
                throw new KeyNotFoundException($"MetricsUser with ID {id} does not exist");
            }

            _context.MetricsUsers.Remove(metrics);
            await _context.SaveChangesAsync();
        }
    }
}

