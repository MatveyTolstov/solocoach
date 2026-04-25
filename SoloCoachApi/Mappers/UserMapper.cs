using SoloCoachApi.ModelDto;
using SoloCoachApi.Models;

namespace SoloCoachApi.Mappers
{
    public class UserMapper
    {
        public User ToModel(UserDto dto)
        {
            return new User
            {
                IdUser = dto.IdUser,
                Name = dto.Name,
                Login = dto.Login,
                Email = dto.Email,
                RoleId = dto.RoleId,
                MetricsUserId = dto.MetricsUserId,
                Password = dto.Password ?? string.Empty
            };
        }

        public User ToModel(CreateUserDto dto)
        {
            return new User
            {
                Name = dto.Name,
                Login = dto.Login,
                Password = dto.Password,
                Email = dto.Email,
                RoleId = dto.RoleId,
                MetricsUserId = dto.MetricsUserId
            };
        }

        public UserDto ToDto(User user)
        {
            return new UserDto
            {
                IdUser = user.IdUser,
                Name = user.Name,
                Login = user.Login,
                Email = user.Email,
                RoleId = user.RoleId,
                MetricsUserId = user.MetricsUserId
            };
        }
    }
}
