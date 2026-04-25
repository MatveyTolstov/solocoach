using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto;

/// <summary>Одна строка журнала подходов в рамках завершённой тренировки.</summary>
public class WorkoutLogLineDto
{
    [Range(1, 1000)]
    public int Repetitions { get; set; }

    [Range(1, 100)]
    public int Sets { get; set; }

    [Range(0.0, 500.0)]
    public float Weight { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }
}
