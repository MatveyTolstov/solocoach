using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("goals")]
    public class Goal
    {
        [Key]
        [Column("id_goal")]
        public int IdGoal { get; set; }

        [Column("type_goal")]
        public required string TypeGoal { get; set; }

        [Column("target_weight")]
        public float TargetWeight { get; set; }

        [Column("date_start")]
        public DateTime DateStart { get; set; }

        [Column("date_end")]
        public DateTime DateEnd { get; set; }

        [Column("status")]
        public required string Status { get; set; }
    }
}