namespace SoloCoachApi.ModelDto;

public class CompleteWorkoutSessionResultDto
{
    public required WorkoutUserDto WorkoutUser { get; set; }
    public required List<WorkoutUserLogDto> Logs { get; set; }
}
