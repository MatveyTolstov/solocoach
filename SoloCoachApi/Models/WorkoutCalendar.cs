using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("workout_calendars")]
    public class WorkoutCalendar
    {
        [Key]
        [Column("id_workout_calendar")]
        public int IdWorkoutCalendar { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("workout_id")]
        public int WorkoutId { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(WorkoutId))]
        public virtual Workout Workout { get; set; } = null!;
    }
}
