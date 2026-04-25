using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class MetricsUserService
    {
        private readonly IMetricsUserRepository _metricsUserRepository;

        public MetricsUserService(IMetricsUserRepository metricsUserRepository)
        {
            _metricsUserRepository = metricsUserRepository;
        }

        public Task<MetricsUserDto> GetByIdAsync(int id) => _metricsUserRepository.GetMetricsUserByIdAsync(id);

        public Task<List<MetricsUserDto>> GetAllAsync() => _metricsUserRepository.GetAllMetricsUsersAsync();

        public Task<MetricsUserDto> CreateAsync(MetricsUserDto dto) => _metricsUserRepository.CreateMetricsUserAsync(dto);

        public Task<MetricsUserDto> UpdateAsync(MetricsUserDto dto) => _metricsUserRepository.UpdateMetricsUserAsync(dto);

        public Task DeleteAsync(int id) => _metricsUserRepository.DeleteMetricsUserAsync(id);
    }
}

