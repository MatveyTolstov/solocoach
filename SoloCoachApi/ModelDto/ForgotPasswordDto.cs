using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
