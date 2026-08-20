using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.DTOs
{
    public class UpdateExperimentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ExperimentStatus Status { get; set; }
    }
}
