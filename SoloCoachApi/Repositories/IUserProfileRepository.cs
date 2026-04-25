using SoloCoachApi.ModelDto;

namespace SoloCoachApi.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfileDto> GetProfileAsync(int userId);

        Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateUserProfileDto dto);
    }
}
