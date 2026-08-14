using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class ObjectiveStatusDisplay
    {
        public static string GetBadgeClass(ObjectiveStatus status) => "bg-secondary-subtle text-secondary-emphasis border border-secondary-subtle";

        public static string GetText(ObjectiveStatus status) => status switch
        {
            ObjectiveStatus.NotStarted => "Not Started",
            ObjectiveStatus.InProgress => "In Progress",
            ObjectiveStatus.Completed => "Completed",
            _ => status.ToString()
        };
    }
}
