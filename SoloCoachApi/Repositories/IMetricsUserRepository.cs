using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IMetricsUserRepository
    {
        Task<MetricsUserDto> GetMetricsUserByIdAsync(int id);
        Task<List<MetricsUserDto>> GetAllMetricsUsersAsync();
        Task<MetricsUserDto> CreateMetricsUserAsync(MetricsUserDto dto);
        Task<MetricsUserDto> UpdateMetricsUserAsync(MetricsUserDto dto);
        Task DeleteMetricsUserAsync(int id);
    }
}

