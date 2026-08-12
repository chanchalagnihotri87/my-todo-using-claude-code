using System;
using System.Collections.Generic;
using System.Text;
using MyTodo.Domain.Enums;

namespace MyTodo.Domain.Entities
{
    public class Problem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProblemStatus Status { get; set; } = ProblemStatus.Pending;
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int LifeAreaId { get; set; }
        public LifeArea LifeArea { get; set; } = null!;

        public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    }
}
