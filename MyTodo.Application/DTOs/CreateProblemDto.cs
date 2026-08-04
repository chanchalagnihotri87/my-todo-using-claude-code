namespace MyTodo.Application.DTOs
{
    public class CreateProblemDto
    {
        public int LifeAreaId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
