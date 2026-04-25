using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class ExerciseDto
    {
        public int IdExercise { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Complexity { get; set; }

        [Required]
        [Url]
        public required string PictureUrl { get; set; }
    }
}
