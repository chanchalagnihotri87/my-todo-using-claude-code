using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class TodoStatusDisplay
    {
        public static string GetBadgeClass(TodoStatus status) => status switch
        {
            TodoStatus.Pending => "bg-secondary",
            TodoStatus.InProgress => "bg-info text-dark",
            TodoStatus.Completed => "bg-success",
            _ => "bg-secondary"
        };

        public static string GetText(TodoStatus status) => status switch
        {
            TodoStatus.Pending => "Pending",
            TodoStatus.InProgress => "In Progress",
            TodoStatus.Completed => "Completed",
            _ => status.ToString()
        };
    }
}
