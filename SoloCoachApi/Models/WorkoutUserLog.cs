using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("workout_user_logs")]
    public class WorkoutUserLog
    {
        [Key]
        [Column("id_workout_user_log")]
        public int IdWorkoutUserLog { get; set; }

        [Column("workout_user_id")]
        public int WorkoutUserId { get; set; }

        [Column("workout_id")]
        public int WorkoutId { get; set; }

        [Column("repetitions")]
        public int Repetitions { get; set; }

        [Column("sets")]
        public int Sets { get; set; }

        [Column("weight")]
        public float Weight { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [ForeignKey(nameof(WorkoutUserId))]
        public virtual WorkoutUser WorkoutUser { get; set; } = null!;

        [ForeignKey(nameof(WorkoutId))]
        public virtual Workout Workout { get; set; } = null!;
    }
}
