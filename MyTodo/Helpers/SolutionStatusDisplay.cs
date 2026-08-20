using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Helpers
{
    public static class SolutionStatusDisplay
    {
        public static string GetBadgeClass(SolutionStatus status) => "bg-secondary-subtle text-secondary-emphasis border border-secondary-subtle";

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
