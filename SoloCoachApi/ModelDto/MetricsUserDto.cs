using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto
{
    /// <summary>
    /// Либо указывается существующий <see cref="GoalId"/> (каталог),
    /// либо пара <see cref="GoalType"/> + <see cref="TargetWeight"/> — сервер создаст/обновит одну цель пользователя.
    /// </summary>
    public class MetricsUserDto : IValidatableObject
    {
        public int IdMetricsUser { get; set; }

        [Range(50.0, 300.0, ErrorMessage = "Height must be between 50 and 300 cm")]
        public float Height { get; set; }

        // Guard rail: avoid extreme/unhealthy values even if client sends them.
        // If you need different limits, change them here and in goal validation in UserProfileRepository.
        [Range(35.0, 250.0, ErrorMessage = "Weight must be between 35 and 250 kg")]
        public float Weight { get; set; }

        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150")]
        public int Age { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Gender { get; set; }

        [MaxLength(50)]
        public string? ExperienceLevel { get; set; }

        [MaxLength(50)]
        public string? ActivityLevel { get; set; }

        /// <summary>
        /// Existing goal id from catalog/personal goals table.
        /// If null/0, server can create a personal goal using <see cref="GoalType"/> + <see cref="TargetWeight"/>.
        /// </summary>
        public int? GoalId { get; set; }

        /// <summary>One of: Похудение, Набор мышечной массы, Повышение выносливости, Удержание веса.</summary>
        [MaxLength(100)]
        public string? GoalType { get; set; }

        /// <summary>Desired target weight in kg for personal goal creation.</summary>
        [Range(35.0, 250.0, ErrorMessage = "TargetWeight must be between 35 and 250 kg")]
        public float? TargetWeight { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasCatalogGoal = GoalId.HasValue && GoalId.Value > 0;
            var hasPersonal = !string.IsNullOrWhiteSpace(GoalType) && TargetWeight.HasValue;

            if (!hasCatalogGoal && !hasPersonal)
            {
                yield return new ValidationResult(
                    "Укажите либо goalId (существующая цель), либо goalType и targetWeight.",
                    [nameof(GoalId), nameof(GoalType), nameof(TargetWeight)]);
            }
        }
    }
}
