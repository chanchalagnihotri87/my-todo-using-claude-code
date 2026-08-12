namespace MyTodo.Models
{
    public class ReorderFocusRequest
    {
        public int Id { get; set; }
        public bool IsTwentyPercent { get; set; }
        public List<int> OrderedIds { get; set; } = new();
    }
}
