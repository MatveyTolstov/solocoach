using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("exercise")]
    public class Exercise
    {
        [Key]
        [Column("id_exercise")]
        public int IdExercise { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("complexity")]
        public string? Complexity { get; set; }

        [Column("picture_url")]
        public required string PictureUrl { get; set; }

        [Column("video_url")]
        public string? VideoUrl { get; set; }

        public virtual ICollection<ExerciseGroupsMuscle> ExerciseGroupsMuscles { get; set; } = new List<ExerciseGroupsMuscle>();
    }
}
