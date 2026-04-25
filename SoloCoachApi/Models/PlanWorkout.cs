using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("plan_workouts")]
    public class PlanWorkout
    {
        [Key]
        [Column("id_plan_workouts")]
        public int IdPlanWorkout { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("workout_id")]
        public int WorkoutId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(WorkoutId))]
        public virtual Workout Workout { get; set; } = null!;

        [Column("status")]
        public required string Status { get; set; }

        [Column("source")]
        public string? Source { get; set; }
    }
}
