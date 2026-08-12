using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class ObjectiveStatusDisplay
    {
        public static string GetBadgeClass(ObjectiveStatus status) => status switch
        {
            ObjectiveStatus.NotStarted => "bg-secondary",
            ObjectiveStatus.InProgress => "bg-warning text-dark",
            ObjectiveStatus.Completed => "bg-success",
            _ => "bg-secondary"
        };

        public static string GetText(ObjectiveStatus status) => status switch
        {
            ObjectiveStatus.NotStarted => "Not Started",
            ObjectiveStatus.InProgress => "In Progress",
            ObjectiveStatus.Completed => "Completed",
            _ => status.ToString()
        };
    }
}
