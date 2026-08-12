using System;
using MyTodo.Domain.Enums;

namespace MyTodo.Domain.Entities
{
    public class Objective
    {
        public int Id { get; set; }
        public int SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public ObjectiveStatus Status { get; set; } = ObjectiveStatus.NotStarted;
        public DateTime? CompletedAt { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
