using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class ProfileService
    {
        private readonly IUserProfileRepository _profileRepository;

        public ProfileService(IUserProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public Task<UserProfileDto> GetProfileAsync(int userId) =>
            _profileRepository.GetProfileAsync(userId);

        public Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateUserProfileDto dto) =>
            _profileRepository.UpdateProfileAsync(userId, dto);
    }
}
