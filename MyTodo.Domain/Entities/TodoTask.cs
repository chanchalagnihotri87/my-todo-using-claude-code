using MyTodo.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTodo.Domain.Entities
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TodoStatus Status { get; set; } = TodoStatus.Pending;
        public DateTime? CompletedAt { get; set; }
        public int ObjectiveId { get; set; }
        public Objective Objective { get; set; } = null!;
        public int? SprintId {  get; set; }
        public Sprint? Sprint { get; set; }
        public bool AddedInTodoList { get; set; }
        public DateTime TodoDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Todo? Todo { get; set; }
    }
}
