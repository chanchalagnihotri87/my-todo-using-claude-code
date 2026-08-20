using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.DTOs
{
    public class UpdateProblemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProblemStatus Status { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
    }
}
