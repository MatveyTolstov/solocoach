using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class GroupsMuscleMapper
    {
        public GroupsMuscle ToModel(GroupsMuscleDto dto)
        {
            return new GroupsMuscle
            {
                IdGroupsMuscle = dto.IdGroupsMuscle,
                Name = dto.Name
            };
        }

        public GroupsMuscleDto ToDto(GroupsMuscle groupsMuscle)
        {
            return new GroupsMuscleDto
            {
                IdGroupsMuscle = groupsMuscle.IdGroupsMuscle,
                Name = groupsMuscle.Name
            };
        }
    }
}
