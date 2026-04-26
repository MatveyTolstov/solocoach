namespace SoloCoachApi.ModelDto
{
    public class AiRecommendationItemDto
    {
        public required List<WorkoutDto> Workouts { get; set; }
        public string Reason { get; set; } = null!;
        public string Advice { get; set; } = null!;
    }
}
