using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("application_logs")]
    public class ApplicationLog
    {
        [Key]
        [Column("id_application_log")]
        public int IdApplicationLog { get; set; }

        [Column("action")]
        [Required]
        public required string Action { get; set; } // "CREATE_USER", "LOGIN", "UPDATE_WORKOUT", etc

        [Column("entity_type")]
        [Required]
        public required string EntityType { get; set; } // "User", "Workout", "Exercise", etc

        [Column("entity_id")]
        public int? EntityId { get; set; } // ID сущности которая изменялась

        [Column("user_id")]
        public int? UserId { get; set; } // Кто это сделал

        [Column("details")]
        [Required]
        public required string Details { get; set; } // JSON с деталями действия

        [Column("created_at")]
        [Required]
        public required DateTime CreatedAt { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string? Status { get; set; } // "SUCCESS", "ERROR"

        [Column("error_message")]
        public string? ErrorMessage { get; set; }
    }
}
