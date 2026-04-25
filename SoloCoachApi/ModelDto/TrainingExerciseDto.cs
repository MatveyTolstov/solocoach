using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class TrainingExerciseDto
    {
        public int IdTrainingExercise { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "WorkoutId must be greater than 0")]
        public int WorkoutId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ExerciseId must be greater than 0")]
        public int ExerciseId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ExecutionOrder must be greater than 0")]
        public int ExecutionOrder { get; set; }

        [Range(1, 1000, ErrorMessage = "Repetitions must be between 1 and 1000")]
        public int Repetitions { get; set; }

        [Range(1, 100, ErrorMessage = "Sets must be between 1 and 100")]
        public int Sets { get; set; }

        [Range(0, 3600, ErrorMessage = "RestTime must be between 0 and 3600 seconds")]
        public int RestTime { get; set; }
    }
}
