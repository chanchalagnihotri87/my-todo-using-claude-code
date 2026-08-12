namespace MyTodo.Models
{
    public class UpdateObjectiveStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
