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
    }
}
