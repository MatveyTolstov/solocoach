namespace SoloCoachApi.ModelDto
{
    /// <summary>
    /// Профиль: User + связанные MetricsUser и Goal (без отдельной сущности истории веса).
    /// Текущий вес — <see cref="MetricsUserDto.Weight"/>, целевой — <see cref="GoalDto.TargetWeight"/>.
    /// </summary>
    public class UserProfileDto
    {
        public int IdUser { get; set; }
        public required string Name { get; set; }
        public required string Login { get; set; }
        public required string Email { get; set; }
        public int? MetricsUserId { get; set; }
        public MetricsUserDto? Metrics { get; set; }
        public GoalDto? CurrentGoal { get; set; }
    }
}
