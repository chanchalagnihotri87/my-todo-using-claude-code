using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Helpers
{
    public static class ProblemStatusDisplay
    {
        public static string GetBadgeClass(ProblemStatus status) => status switch
        {
            ProblemStatus.Pending => "bg-secondary-subtle text-secondary-emphasis border border-secondary-subtle",
            ProblemStatus.WorkingOnIt => "bg-warning-subtle text-warning-emphasis border border-warning-subtle",
            ProblemStatus.Resolved => "bg-success-subtle text-success-emphasis border border-success-subtle",
            ProblemStatus.Discarded => "bg-danger-subtle text-danger-emphasis border border-danger-subtle",
            _ => "bg-secondary-subtle text-secondary-emphasis border border-secondary-subtle"
        };

        public static string GetText(ProblemStatus status) => status switch
        {
            ProblemStatus.Pending => "Pending",
            ProblemStatus.WorkingOnIt => "Working on it",
            ProblemStatus.Resolved => "Resolved",
            ProblemStatus.Discarded => "Discarded",
            _ => status.ToString()
        };
    }
}
