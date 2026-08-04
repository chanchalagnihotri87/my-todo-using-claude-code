using System.ComponentModel.DataAnnotations;

namespace MyTodo.Models
{
    public class CreateLifeAreaViewModel
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
