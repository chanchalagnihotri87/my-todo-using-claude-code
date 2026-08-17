using System;
using System.Collections.Generic;
using System.Text;

namespace MyTodo.Domain.Entities
{
    public class Todo
    {
        public int Id { get; set; }
        public int TodoTaskId { get; set; }
        public TodoTask TodoTask { get; set; } = null!;
        public DateOnly TodoDate { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
        public bool IsFrog { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

