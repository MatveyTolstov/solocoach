using System.ComponentModel.DataAnnotations;

namespace SoloCoachApi.ModelDto;

public class ExerciseListQuery
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    [MaxLength(200)]
    public string? Search { get; set; }

    [MaxLength(50)]
    public string? Complexity { get; set; }

    [MaxLength(100)]
    public string? MuscleGroup { get; set; }
}
