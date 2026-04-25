using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class GroupsMuscleService
    {
        private readonly IGroupsMuscleRepository _groupsMuscleRepository;

        public GroupsMuscleService(IGroupsMuscleRepository groupsMuscleRepository)
        {
            _groupsMuscleRepository = groupsMuscleRepository;
        }

        public Task<GroupsMuscleDto> GetByIdAsync(int id) => _groupsMuscleRepository.GetGroupsMuscleByIdAsync(id);

        public Task<List<GroupsMuscleDto>> GetAllAsync() => _groupsMuscleRepository.GetAllGroupsMusclesAsync();

        public Task<GroupsMuscleDto> CreateAsync(GroupsMuscleDto dto) => _groupsMuscleRepository.CreateGroupsMuscleAsync(dto);

        public Task<GroupsMuscleDto> UpdateAsync(GroupsMuscleDto dto) => _groupsMuscleRepository.UpdateGroupsMuscleAsync(dto);

        public Task DeleteAsync(int id) => _groupsMuscleRepository.DeleteGroupsMuscleAsync(id);
    }
}

