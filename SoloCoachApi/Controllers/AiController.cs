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
    public class AiController : ControllerBase
    {
        private readonly AiService _aiService;

        public AiController(AiService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet("recommend")]
        public async Task<ActionResult<AiRecommendationResponseDto>> Recommend()
        {
            var userId = this.User.GetUserId();
            var result = await _aiService.GetRecommendationsAsync(userId);
            return Ok(result);
        }
    }
}
