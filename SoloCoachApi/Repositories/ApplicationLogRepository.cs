namespace SoloCoachApi.Repositories
{
    using SoloCoachApi.Models;
    using SoloCoachApi.DataBase;
    using Microsoft.EntityFrameworkCore;

    public interface IApplicationLogRepository
    {
        Task<List<ApplicationLog>> GetAllAsync();
        Task<ApplicationLog?> GetByIdAsync(int id);
        Task CreateAsync(ApplicationLog log);
        Task UpdateAsync(ApplicationLog log);
        Task DeleteAllAsync();
    }

    public class ApplicationLogRepository : IApplicationLogRepository
    {
        private readonly ApplicationContext _context;

        public ApplicationLogRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationLog>> GetAllAsync()
        {
            return await _context.ApplicationLogs
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApplicationLog?> GetByIdAsync(int id)
        {
            return await _context.ApplicationLogs.FirstOrDefaultAsync(l => l.IdApplicationLog == id);
        }

        public async Task CreateAsync(ApplicationLog log)
        {
            log.CreatedAt = DateTime.UtcNow;
            _context.ApplicationLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationLog log)
        {
            _context.ApplicationLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync()
        {
            var allLogs = await _context.ApplicationLogs.ToListAsync();
            _context.ApplicationLogs.RemoveRange(allLogs);
            await _context.SaveChangesAsync();
        }
    }
}
