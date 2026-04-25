using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("workouts")]
    public class Workout
    {
        [Key]
        [Column("id_workout")]
        public int IdWorkout { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public required string Description { get; set; }

        [Column("duration")]
        public int Duration { get; set; }

        [Column("complexity")]
        public required string Complexity { get; set; }

        [Column("type_workout")]
        public required string TypeWorkout { get; set; }
    }
}
