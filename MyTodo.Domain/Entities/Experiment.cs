using MyTodo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTodo.Domain.Entities
{
    public class Experiment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SolutionId { get; set; }
        public Solution Solution { get; set; } = null!;
        public ExperimentStatus Status { get; set; } = ExperimentStatus.Innovation;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
