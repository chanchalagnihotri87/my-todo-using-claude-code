using MyTodo.Domain.Enums;

namespace MyTodo.Application.DTOs
{
    public class SolutionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProblemId { get; set; }
        public bool IsTwentyPercent { get; set; }
        public int SortOrder { get; set; }
        public SolutionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalObjectivesCount { get; set; }
        public int CompletedObjectivesCount { get; set; }
    }
}
