using System.ComponentModel.DataAnnotations;

namespace MyTodo.Models
{
    public class CreateObjectiveViewModel
    {
        [Required]
        public int SolutionId { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;
    }
}
