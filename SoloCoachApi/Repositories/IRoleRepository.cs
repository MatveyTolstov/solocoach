using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IRoleRepository
    {
        Task<RoleDto> GetRoleByIdAsync(int id);
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> CreateRoleAsync(RoleDto dto);
        Task<RoleDto> UpdateRoleAsync(RoleDto dto);
        Task DeleteRoleAsync(int id);
    }
}

