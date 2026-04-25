using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IGroupsMuscleRepository
    {
        Task<GroupsMuscleDto> GetGroupsMuscleByIdAsync(int id);
        Task<List<GroupsMuscleDto>> GetAllGroupsMusclesAsync();
        Task<GroupsMuscleDto> CreateGroupsMuscleAsync(GroupsMuscleDto dto);
        Task<GroupsMuscleDto> UpdateGroupsMuscleAsync(GroupsMuscleDto dto);
        Task DeleteGroupsMuscleAsync(int id);
    }
}

