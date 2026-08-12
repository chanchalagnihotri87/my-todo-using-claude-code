namespace MyTodo.Application.DTOs
{
    public class CreateObjectiveDto
    {
        public int SolutionId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
