namespace MyTodo.Application.DTOs
{
    public class CreateTodoTaskDto
    {
        public int ObjectiveId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
