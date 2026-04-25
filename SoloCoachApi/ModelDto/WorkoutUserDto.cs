using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class WorkoutUserDto
    {
        public int IdWorkoutUser { get; set; }

        /// <summary>При POST игнорируется: выставляется из JWT в контроллере.</summary>
        public int UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "WorkoutId must be greater than 0")]
        public int WorkoutId { get; set; }

        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int Duration { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }
    }
}
