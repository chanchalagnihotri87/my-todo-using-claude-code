namespace MyTodo.Models
{
    public class ReorderObjectivesFocusRequest
    {
        public int Id { get; set; }
        public bool IsTwentyPercent { get; set; }
        public List<int> OrderedIds { get; set; } = new();
    }
}
