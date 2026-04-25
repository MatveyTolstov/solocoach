using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id_user")]
        public int IdUser { get; set; }

        [Column("name")]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Column("login")]
        [MaxLength(50)]
        [Required]
        public required string Login { get; set; }

        [Column("password")]
        [Required]
        public required string Password { get; set; }

        [Column("email")]
        [EmailAddress]
        [MaxLength(100)]
        [Required]
        public string Email { get; set; } = null!;

        [Column("role_id")]
        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;

        [Column("metrics_user_id")]
        public int? MetricsUserId { get; set; }

        [ForeignKey(nameof(MetricsUserId))]
        public virtual MetricsUser? MetricsUser { get; set; }
    }
}
