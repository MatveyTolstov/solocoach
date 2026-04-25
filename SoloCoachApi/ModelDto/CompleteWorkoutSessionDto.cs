using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto;

public class CompleteWorkoutSessionDto
{
    [Range(1, int.MaxValue)]
    public int WorkoutId { get; set; }

    [Range(1, 600)]
    public int Duration { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    /// <summary>Если указан — запись календаря помечается как «Выполнена».</summary>
    public int? CalendarEntryId { get; set; }

    public List<WorkoutLogLineDto> Logs { get; set; } = [];
}
