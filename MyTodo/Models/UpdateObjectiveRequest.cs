namespace MyTodo.Models
{
    public class UpdateObjectiveRequest
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
