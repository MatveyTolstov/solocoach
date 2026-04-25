using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class GoalService
    {
        private readonly IGoalRepository _goalRepository;

        public GoalService(IGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }

        public Task<GoalDto> GetByIdAsync(int id) => _goalRepository.GetGoalByIdAsync(id);

        public Task<List<GoalDto>> GetAllAsync() => _goalRepository.GetAllGoalsAsync();

        public Task<GoalDto> CreateAsync(GoalDto dto) => _goalRepository.CreateGoalAsync(dto);

        public Task<GoalDto> UpdateAsync(GoalDto dto) => _goalRepository.UpdateGoalAsync(dto);

        public Task DeleteAsync(int id) => _goalRepository.DeleteGoalAsync(id);
    }
}

