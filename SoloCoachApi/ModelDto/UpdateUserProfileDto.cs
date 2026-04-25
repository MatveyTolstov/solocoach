using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    public class UpdateUserProfileDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Login { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MinLength(6)]
        [MaxLength(100)]
        public string? Password { get; set; }

        /// <summary>Если задано — создаётся или обновляется блок метрик пользователя.</summary>
        public MetricsUserDto? Metrics { get; set; }
    }
}
