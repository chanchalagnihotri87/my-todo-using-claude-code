namespace MyTodo.Application.DTOs
{
    public class CreateSolutionDto
    {
        public int ProblemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsTwentyPercent { get; set; }
    }
}
