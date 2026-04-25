using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class PlanWorkoutService
    {
        private readonly IPlanWorkoutRepository _planWorkoutRepository;

        public PlanWorkoutService(IPlanWorkoutRepository planWorkoutRepository)
        {
            _planWorkoutRepository = planWorkoutRepository;
        }

        public Task<PlanWorkoutDto> GetByIdAsync(int id) => _planWorkoutRepository.GetPlanWorkoutByIdAsync(id);

        public Task<List<PlanWorkoutDto>> GetAllAsync() => _planWorkoutRepository.GetAllPlanWorkoutsAsync();

        public Task<PlanWorkoutDto> CreateAsync(PlanWorkoutDto dto) => _planWorkoutRepository.CreatePlanWorkoutAsync(dto);

        public Task<PlanWorkoutDto> UpdateAsync(PlanWorkoutDto dto) => _planWorkoutRepository.UpdatePlanWorkoutAsync(dto);

        public Task DeleteAsync(int id) => _planWorkoutRepository.DeletePlanWorkoutAsync(id);
    }
}

