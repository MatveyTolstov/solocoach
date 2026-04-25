using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("metrics_users")]
    public class MetricsUser
    {
        [Key]
        [Column("id_metrics_user")]
        public int IdMetricsUser { get; set; }

        [Column("height")]
        public float Height { get; set; }

        [Column("weight")]
        public float Weight { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("gender")]
        public required string Gender { get; set; }

        [Column("experience_level")]
        public string? ExperienceLevel { get; set; }

        [Column("activity_level")]
        public string? ActivityLevel { get; set; }

        public int GoalId { get; set; }

        [ForeignKey(nameof(GoalId))]
        public Goal? Goal { get; set; }

    }
}
