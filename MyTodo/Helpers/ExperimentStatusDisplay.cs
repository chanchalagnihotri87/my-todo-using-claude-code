using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class ExperimentStatusDisplay
    {
        public static string GetBadgeClass(ExperimentStatus status) => "bg-secondary-subtle text-secondary-emphasis border border-secondary-subtle";

        public static string GetText(ExperimentStatus status) => status switch
        {
            ExperimentStatus.Innovation => "Innovation",
            ExperimentStatus.Verifying => "Verifying",
            ExperimentStatus.Verified => "Verified",
            ExperimentStatus.AddedInSOP => "Added In SOP",
            ExperimentStatus.Discarded => "Discarded",
            _ => status.ToString()
        };
    }
}
