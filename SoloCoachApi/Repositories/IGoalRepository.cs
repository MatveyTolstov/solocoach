using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IGoalRepository
    {
        Task<GoalDto> GetGoalByIdAsync(int id);
        Task<List<GoalDto>> GetAllGoalsAsync();
        Task<GoalDto> CreateGoalAsync(GoalDto dto);
        Task<GoalDto> UpdateGoalAsync(GoalDto dto);
        Task DeleteGoalAsync(int id);
    }
}

