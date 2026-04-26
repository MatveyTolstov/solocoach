namespace SoloCoachApi.ModelDto
{
    public class AiWorkoutRecommendationDto
    {
        public WorkoutDto Workout { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
