using MyTodo.Domain.Enums;

namespace MyTodo.Models
{
    public class UpdateTodoTaskRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TodoStatus Status { get; set; }
        public int? SprintId { get; set; }
    }
}
