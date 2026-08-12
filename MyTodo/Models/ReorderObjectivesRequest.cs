namespace MyTodo.Models
{
    public class ReorderObjectivesRequest
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<int> OrderedIds { get; set; } = new();
    }
}
