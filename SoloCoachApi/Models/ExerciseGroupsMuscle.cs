using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("exercise_groups_muscles")]
    public class ExerciseGroupsMuscle
    {
        [Key]
        [Column("id_exercise_groups_muscle")]
        public int IdExerciseGroupsMuscle { get; set; }

        [Column("exercise_id")]
        public int ExerciseId { get; set; }

        [Column("groups_muscles_id")]
        public int GroupsMusclesId { get; set; }

        [ForeignKey(nameof(ExerciseId))]
        public virtual Exercise Exercise { get; set; } = null!;

        [ForeignKey(nameof(GroupsMusclesId))]
        public virtual GroupsMuscle GroupsMuscle { get; set; } = null!;
    }
}
