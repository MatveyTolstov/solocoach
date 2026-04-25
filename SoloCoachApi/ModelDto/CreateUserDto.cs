using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class CreateUserDto
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Login { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public required string Password { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0")]
        public int RoleId { get; set; }

        public int? MetricsUserId { get; set; }
    }
}
