using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class SolutionStatusDisplay
    {
        public static string GetBadgeClass(SolutionStatus status) => status switch
        {
            SolutionStatus.Planned => "bg-secondary",
            SolutionStatus.Verifying => "bg-warning text-dark",
            SolutionStatus.Verified => "bg-info text-dark",
            SolutionStatus.AddedInRoutine => "bg-primary",
            SolutionStatus.BecomeSecondNature => "bg-success",
            SolutionStatus.Discarded => "bg-danger",
            _ => "bg-secondary"
        };

        public static string GetText(SolutionStatus status) => status switch
        {
            SolutionStatus.Planned => "Planned",
            SolutionStatus.Verifying => "Verifying",
            SolutionStatus.Verified => "Verified",
            SolutionStatus.AddedInRoutine => "Added in Routine",
            SolutionStatus.BecomeSecondNature => "Become Second Nature",
            SolutionStatus.Discarded => "Discarded",
            _ => status.ToString()
        };
    }
}
