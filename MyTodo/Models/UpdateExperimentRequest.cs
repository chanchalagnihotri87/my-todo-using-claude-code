using System.ComponentModel.DataAnnotations;

namespace MyTodo.Models
{
    public class UpdateExperimentRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
