using MyTodo.Domain.Enums;

namespace MyTodo.Application.DTOs
{
    public class TodoTaskDto
    {
        public int Id { get; set; }
        public int ObjectiveId { get; set; }
        public string Name { get; set; } = string.Empty;
        public TodoStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? SprintId { get; set; }
        public string? SprintName { get; set; }
        public string? ObjectiveText { get; set; }
        public int? TodoId { get; set; }
        public DateOnly? TodoDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
