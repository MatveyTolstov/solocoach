using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class GoalDto
    {
        public int IdGoal { get; set; }

        [Required]
        [MaxLength(100)]
        public required string TypeGoal { get; set; }

        [Range(0.0, 500.0, ErrorMessage = "TargetWeight must be between 0 and 500")]
        public float TargetWeight { get; set; }

        [Required]
        public DateTime DateStart { get; set; }

        [Required]
        public DateTime DateEnd { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; }
    }
}
