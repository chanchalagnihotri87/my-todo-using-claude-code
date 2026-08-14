using MyTodo.Domain.Enums;

namespace MyTodo.Application.DTOs
{
    public class ObjectiveDto
    {
        public int Id { get; set; }
        public int SolutionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public ObjectiveStatus Status { get; set; }
        public bool IsTwentyPercent { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
