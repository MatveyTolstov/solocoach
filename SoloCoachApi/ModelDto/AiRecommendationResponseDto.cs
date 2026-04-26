namespace SoloCoachApi.ModelDto
{
    public class AiRecommendationResponseDto
    {
        public List<AiWorkoutRecommendationDto> Recommendations { get; set; } = [];
        public string Advice { get; set; } = string.Empty;
        public string Disclaimer { get; set; } = "Рекомендации носят информационный характер и не заменяют консультацию врача или тренера.";
    }
}
