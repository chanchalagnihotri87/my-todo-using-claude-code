using MyTodo.Domain.Enums;

namespace MyTodo.Application.DTOs
{
    public class UpdateObjectiveDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public ObjectiveStatus Status { get; set; }
    }
}
