using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.DTOs
{
    public class ExperimentDto
    {
        public int Id { get; set; }
        public int SolutionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ExperimentStatus Status { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
