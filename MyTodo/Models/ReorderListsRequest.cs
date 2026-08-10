namespace MyTodo.Models
{
    public class ReorderListsRequest
    {
        public List<string> OrderedStatuses { get; set; } = new();
    }
}
