namespace SoloCoachApi.ModelDto
{
    public class ApplicationLogDto
    {
        public int IdApplicationLog { get; set; }
        public required string Action { get; set; }
        public required string EntityType { get; set; }
        public int? EntityId { get; set; }
        public int? UserId { get; set; }
        public required string Details { get; set; }
        public required DateTime CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
