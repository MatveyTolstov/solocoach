using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("training_exercises")]
    public class TrainingExercise
    {
        [Key]
        [Column("id_training_exercise")]
        public int IdTrainingExercise { get; set; }

        [Column("workout_id")]
        public int WorkoutId { get; set; }

        [Column("exercise_id")]
        public int ExerciseId { get; set; }

        [Column("execution_order")]
        public int ExecutionOrder { get; set; }

        [Column("repetitions")]
        public int Repetitions { get; set; }

        [Column("sets")]
        public int Sets { get; set; }

        [Column("rest_time")]
        public int RestTime { get; set; }

        [ForeignKey(nameof(WorkoutId))]
        public virtual Workout Workout { get; set; } = null!;

        [ForeignKey(nameof(ExerciseId))]
        public virtual Exercise Exercise { get; set; } = null!;
    }
}
