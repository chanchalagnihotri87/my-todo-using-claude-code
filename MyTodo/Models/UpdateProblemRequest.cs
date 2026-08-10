namespace MyTodo.Models
{
    public class UpdateProblemRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }
        public bool IsImportant { get; set; }
    }
}
