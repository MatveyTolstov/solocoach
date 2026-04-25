using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class RoleDto
    {
        public int IdRole { get; set; }

        [Required]
        [MaxLength(50)]
        public required string RoleName { get; set; }
    }
}
