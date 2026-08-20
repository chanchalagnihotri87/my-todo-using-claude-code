using MyTodo.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTodo.Domain.Entities
{
    public class Solution
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProblemId { get; set; }
        public Problem Problem { get; set; } = null!;
        public bool IsTwentyPercent { get; set; }
        public int SortOrder { get; set; }
        public SolutionStatus Status { get; set; } = SolutionStatus.Planned;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Objective> Objectives { get; set; } = new List<Objective>();
    }
}
