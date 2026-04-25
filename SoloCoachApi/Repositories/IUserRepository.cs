using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IUserRepository
    {
        Task<UserDto> GetUserByIdAsync(int id);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(UserDto dto);
        Task<UserDto> UpdateUserAsync(UserDto dto);
        Task DeleteUserAsync(int id);
        Task<bool> IsEmailTakenAsync(int id, string email);
    }
}

