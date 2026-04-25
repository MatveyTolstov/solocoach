using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("workout_users")]
    public class WorkoutUser
    {
        [Key]
        [Column("id_workout_users")]
        public int IdWorkoutUser { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("workout_id")]
        public int WorkoutId { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(WorkoutId))]
        public virtual Workout Workout { get; set; } = null!;
    }
}
