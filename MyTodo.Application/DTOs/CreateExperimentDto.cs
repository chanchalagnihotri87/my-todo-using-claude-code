namespace MyTodo.Application.DTOs
{
    public class CreateExperimentDto
    {
        public int SolutionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
