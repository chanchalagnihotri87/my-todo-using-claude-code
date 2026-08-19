using System.ComponentModel.DataAnnotations;
using MyTodo.Domain.Enums;

namespace MyTodo.Models
{
    public class UpdateTodoTaskRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public TodoStatus Status { get; set; }
        public int? SprintId { get; set; }
    }
}
