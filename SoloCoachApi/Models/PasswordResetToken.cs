using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("password_reset_token")]
    public class PasswordResetToken
    {
        [Key]
        [Column("id_password_reset_token")]
        public int IdPasswordResetToken { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("token_hash")]
        public string TokenHash { get; set; }
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}