using System.ComponentModel.DataAnnotations;

namespace MyTodo.Models
{
    public class CreateSolutionViewModel
    {
        public int ProblemId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "20% Solution")]
        public bool IsTwentyPercent { get; set; }
    }
}
