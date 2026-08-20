using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.DTOs
{
    public class UpdateTodoTaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TodoStatus Status { get; set; }
        public int? SprintId { get; set; }
    }
}
