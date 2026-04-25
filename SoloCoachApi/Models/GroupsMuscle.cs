using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("groups_muscles")]
    public class GroupsMuscle
    {
        [Key]
        [Column("id_groups_muscle")]
        public int IdGroupsMuscle { get; set; }

        [Column("name")]
        public required string Name { get; set; }
    }
}
