using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IPlanWorkoutRepository
    {
        Task<PlanWorkoutDto> GetPlanWorkoutByIdAsync(int id);
        Task<List<PlanWorkoutDto>> GetAllPlanWorkoutsAsync();
        Task<PlanWorkoutDto> CreatePlanWorkoutAsync(PlanWorkoutDto dto);
        Task<PlanWorkoutDto> UpdatePlanWorkoutAsync(PlanWorkoutDto dto);
        Task DeletePlanWorkoutAsync(int id);
    }
}

