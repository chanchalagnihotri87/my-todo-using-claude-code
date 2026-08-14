namespace MyTodo.Models
{
    public class UpdateTodoDateRequest
    {
        public int Id { get; set; }
        public DateOnly TodoDate { get; set; }
    }
}
