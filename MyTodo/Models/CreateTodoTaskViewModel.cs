using System.ComponentModel.DataAnnotations;

namespace MyTodo.Models
{
    public class CreateTodoTaskViewModel
    {
        [Required]
        public int ObjectiveId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
    }
}
