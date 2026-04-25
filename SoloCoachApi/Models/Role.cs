using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoloCoachApi.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id_role")]
        public int IdRole { get; set; }
        [Column("role_name")]
        [MaxLength(50)]
        public required string RoleName { get; set; }
    }
}
