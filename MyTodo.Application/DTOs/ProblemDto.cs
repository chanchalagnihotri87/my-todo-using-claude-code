using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.DTOs
{
    public class ProblemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProblemStatus Status { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LifeAreaId { get; set; }
    }
}
