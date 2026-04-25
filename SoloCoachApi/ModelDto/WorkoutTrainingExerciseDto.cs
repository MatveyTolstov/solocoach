namespace SoloCoachApi.ModelDto
{
    /// <summary>
    /// Строка из training_exercises с полными данными упражнения из exercise.
    /// </summary>
    public class WorkoutTrainingExerciseDto
    {
        public int IdTrainingExercise { get; set; }

        public int ExecutionOrder { get; set; }

        public int Repetitions { get; set; }

        public int Sets { get; set; }

        public int RestTime { get; set; }

        public required ExerciseDto Exercise { get; set; }
    }
}
