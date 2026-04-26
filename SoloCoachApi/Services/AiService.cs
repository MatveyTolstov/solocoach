using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using SoloCoachApi.ModelDto;
using SoloCoachApi.Repositories;

namespace SoloCoachApi.Services
{
    public class AiService
    {
        private readonly HttpClient _http;
        private readonly ProfileService _profileService;
        private readonly WorkoutService _workoutService;
        private readonly IWorkoutUserRepository _workoutUserRepository;
        private readonly string _apiKey;
        private readonly string _model;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public AiService(
            HttpClient http,
            ProfileService profileService,
            WorkoutService workoutService,
            IWorkoutUserRepository workoutUserRepository,
            IConfiguration configuration)
        {
            _http = http;
            _profileService = profileService;
            _workoutService = workoutService;
            _workoutUserRepository = workoutUserRepository;
            _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new InvalidOperationException("OpenRouter:ApiKey not configured");
            _model = configuration["OpenRouter:Model"] ?? "anthropic/claude-sonnet-4-6";
        }

        public async Task<AiRecommendationResponseDto> GetRecommendationsAsync(int userId)
        {
            var profile = await _profileService.GetProfileAsync(userId);
            var allWorkouts = await _workoutService.GetAllAsync();
            var history = await _workoutUserRepository.GetByUserIdAsync(userId);

            var recentWorkoutIds = history
                .OrderByDescending(w => w.Date)
                .Take(10)
                .Select(w => w.WorkoutId)
                .ToHashSet();

            var recentWorkoutNames = allWorkouts
                .Where(w => recentWorkoutIds.Contains(w.IdWorkout))
                .Select(w => w.Name)
                .ToList();

            var workoutCatalog = allWorkouts.Select(w => new
            {
                id = w.IdWorkout,
                name = w.Name,
                complexity = w.Complexity,
                type = w.TypeWorkout,
                duration = w.Duration
            });

            var userContext = new
            {
                age = profile.Metrics?.Age,
                gender = profile.Metrics?.Gender,
                height = profile.Metrics?.Height,
                weight = profile.Metrics?.Weight,
                experienceLevel = profile.Metrics?.ExperienceLevel,
                activityLevel = profile.Metrics?.ActivityLevel,
                goal = profile.CurrentGoal?.TypeGoal,
                targetWeight = profile.CurrentGoal?.TargetWeight,
                recentWorkouts = recentWorkoutNames,
                availableWorkouts = workoutCatalog
            };

            var userMessage = $$"""
                Данные пользователя:
                {{JsonSerializer.Serialize(userContext, _jsonOptions)}}

                Выбери 3-5 тренировок из availableWorkouts которые подойдут этому пользователю.
                Ответь строго в JSON формате без лишнего текста:
                {
                  "recommendations": [
                    { "workoutId": <id из списка>, "reason": "<причина на русском>" }
                  ],
                  "advice": "<общий совет по тренировкам на русском>"
                }
                """;

            var raw = await CallOpenRouterAsync(
                systemPrompt: "Ты опытный персональный тренер. Подбираешь тренировки исходя из данных пользователя. Отвечаешь только в указанном JSON формате без markdown.",
                userMessage: userMessage);

            var parsed = JsonSerializer.Deserialize<AiRawResponse>(raw, _jsonOptions)
                ?? throw new InvalidOperationException("AI вернул некорректный ответ");

            var workoutMap = allWorkouts.ToDictionary(w => w.IdWorkout);

            var recommendations = parsed.Recommendations
                .Where(r => workoutMap.ContainsKey(r.WorkoutId))
                .Select(r => new AiWorkoutRecommendationDto
                {
                    Workout = workoutMap[r.WorkoutId],
                    Reason = r.Reason
                })
                .ToList();

            return new AiRecommendationResponseDto
            {
                Recommendations = recommendations,
                Advice = parsed.Advice
            };
        }

        private async Task<string> CallOpenRouterAsync(string systemPrompt, string userMessage)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",   content = userMessage  }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                throw new InvalidOperationException("Превышен лимит запросов к AI. Попробуйте через несколько минут.");

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            // Strip <think>...</think> block (deepseek-r1)
            content = content.Trim();
            var thinkEnd = content.IndexOf("</think>", StringComparison.OrdinalIgnoreCase);
            if (thinkEnd >= 0)
                content = content[(thinkEnd + 8)..].Trim();

            // Strip possible markdown code block
            if (content.StartsWith("```"))
            {
                var start = content.IndexOf('{');
                var end = content.LastIndexOf('}');
                if (start >= 0 && end > start)
                    content = content[start..(end + 1)];
            }

            return content;
        }
    }

    // Internal types for parsing AI JSON response
    internal class AiRawRecommendation
    {
        public int WorkoutId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    internal class AiRawResponse
    {
        public List<AiRawRecommendation> Recommendations { get; set; } = [];
        public string Advice { get; set; } = string.Empty;
    }
}
