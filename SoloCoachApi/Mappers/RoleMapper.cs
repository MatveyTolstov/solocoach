using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class RoleMapper
    {
        public Role ToModel(RoleDto roleDto) => new Role { IdRole = roleDto.IdRole, RoleName = roleDto.RoleName };

        public RoleDto ToDto(Role role) => new RoleDto { IdRole = role.IdRole, RoleName = role.RoleName };
    }
}
