using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public Task<RoleDto> GetByIdAsync(int id) => _roleRepository.GetRoleByIdAsync(id);

        public Task<List<RoleDto>> GetAllAsync() => _roleRepository.GetAllRolesAsync();

        public Task<RoleDto> CreateAsync(RoleDto dto) => _roleRepository.CreateRoleAsync(dto);

        public Task<RoleDto> UpdateAsync(RoleDto dto) => _roleRepository.UpdateRoleAsync(dto);

        public Task DeleteAsync(int id) => _roleRepository.DeleteRoleAsync(id);
    }
}

