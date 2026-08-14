using MyTodo.Domain.Enums;

namespace MyTodo.Application.DTOs
{
    public class TodoDto
    {
        public int Id { get; set; }
        public int TodoTaskId { get; set; }
        public string TodoTaskName { get; set; } = string.Empty;
        public TodoStatus TaskStatus { get; set; }
        public int ObjectiveId { get; set; }
        public string ObjectiveText { get; set; } = string.Empty;
        public string? SprintName { get; set; }
        public DateOnly TodoDate { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
        public bool IsFrog { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
