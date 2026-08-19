using System.ComponentModel.DataAnnotations;

namespace MyTodo.Models
{
    public class UpdateObjectiveRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
