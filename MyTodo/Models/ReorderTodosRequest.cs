namespace MyTodo.Models
{
    public class ReorderTodosRequest
    {
        public List<int> OrderedIds { get; set; } = new();
    }
}
