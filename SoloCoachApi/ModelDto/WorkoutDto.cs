using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class WorkoutDto
    {
        public int IdWorkout { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public required string Description { get; set; }

        [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
        public int Duration { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Complexity { get; set; }

        [Required]
        [MaxLength(50)]
        public required string TypeWorkout { get; set; }

        /// <summary>
        /// Заполняется при GET по id: упражнения тренировки из training_exercises + exercise.
        /// </summary>
        public List<WorkoutTrainingExerciseDto> TrainingExercises { get; set; } = [];
    }
}
