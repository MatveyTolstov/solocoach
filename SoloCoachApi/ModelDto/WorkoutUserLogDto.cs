using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class WorkoutUserLogDto
    {
        public int IdWorkoutUserLog { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "WorkoutUserId must be greater than 0")]
        public int WorkoutUserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "WorkoutId must be greater than 0")]
        public int WorkoutId { get; set; }

        [Range(1, 1000, ErrorMessage = "Repetitions must be between 1 and 1000")]
        public int Repetitions { get; set; }

        [Range(1, 100, ErrorMessage = "Sets must be between 1 and 100")]
        public int Sets { get; set; }

        [Range(0.0, 500.0, ErrorMessage = "Weight must be between 0 and 500 kg")]
        public float Weight { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }
}
