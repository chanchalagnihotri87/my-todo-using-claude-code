using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Helpers
{
    public static class TodoStatusDisplay
    {
        public static string GetBadgeClass(TodoStatus status) => "bg-secondary-subtle text-secondary-emphasis border border-secondary-subtle";

        public static string GetText(TodoStatus status) => status switch
        {
            TodoStatus.Pending => "Pending",
            TodoStatus.InProgress => "In Progress",
            TodoStatus.Completed => "Completed",
            _ => status.ToString()
        };
    }
}
