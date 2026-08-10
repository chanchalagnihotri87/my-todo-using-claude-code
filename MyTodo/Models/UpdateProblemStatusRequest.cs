namespace MyTodo.Models
{
    public class UpdateProblemStatusRequest
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
