using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class ExerciseGroupsMuscleDto
    {
        public int IdExerciseGroupsMuscle { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ExerciseId must be greater than 0")]
        public int ExerciseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "GroupsMusclesId must be greater than 0")]
        public int GroupsMusclesId { get; set; }
    }
}
