using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloCoachApi.Extensions;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Services;

namespace SoloCoachApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ProfileService profileService, ILoggingService loggingService, ILogger<ProfileController> logger)
        {
            _profileService = profileService;
            _loggingService = loggingService;
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMe()
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateMe([FromBody] UpdateUserProfileDto dto)
        {
            if (!TryGetUserId(out var userId))
            {
                return Unauthorized();
            }

            try
            {
                var profile = await _profileService.UpdateProfileAsync(userId, dto);
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_PROFILE", entityType: "Profile", entityId: userId, details: new { name = dto.Name });
                return Ok(profile);
            }
            catch (ArgumentException ex)
            {
                await _loggingService.LogActionAsync(userId: userId, action: "UPDATE_PROFILE", entityType: "Profile", entityId: userId, details: new { name = dto.Name }, status: "ERROR", errorMessage: ex.Message);
                return BadRequest(ex.Message);
            }
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return !string.IsNullOrEmpty(raw) && int.TryParse(raw, out userId) && userId > 0;
        }
    }
}
