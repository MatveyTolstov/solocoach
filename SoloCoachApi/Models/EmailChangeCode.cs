using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("email_change_codes")]
    public class EmailChangeCode
    {
        [Key]
        [Column("id_email_change_code")]
        public int IdEmailChangeCode { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("new_email")]
        [MaxLength(100)]
        public string NewEmail { get; set; } = null!;

        [Column("code")]
        [MaxLength(6)]
        public string Code { get; set; } = null!;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("is_used")]
        public bool IsUsed { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
