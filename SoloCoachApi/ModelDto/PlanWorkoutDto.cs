using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class PlanWorkoutDto
    {
        public int IdPlanWorkout { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
        public int UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "WorkoutId must be greater than 0")]
        public int WorkoutId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; }

        [MaxLength(100)]
        public string? Source { get; set; }
    }
}
