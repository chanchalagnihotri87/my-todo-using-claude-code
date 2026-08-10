using MyTodo.Domain.Enums;

namespace MyTodo.Domain.Entities
{
    public class ProblemStatusOrder
    {
        public int Id { get; set; }
        public ProblemStatus Status { get; set; }
        public int SortOrder { get; set; }
    }
}
