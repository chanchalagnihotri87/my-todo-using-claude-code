using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class ProblemStatusDisplay
    {
        public static string GetBadgeClass(ProblemStatus status) => status switch
        {
            ProblemStatus.Pending => "bg-secondary",
            ProblemStatus.WorkingOnIt => "bg-warning text-dark",
            ProblemStatus.Resolved => "bg-success",
            ProblemStatus.Discarded => "bg-danger",
            _ => "bg-secondary"
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
