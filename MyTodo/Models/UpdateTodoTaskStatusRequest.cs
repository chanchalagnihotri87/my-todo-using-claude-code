using MyTodo.Domain.Enums;

namespace MyTodo.Models
{
    public class UpdateTodoTaskStatusRequest
    {
        public int Id { get; set; }
        public TodoStatus Status { get; set; }
    }
}
