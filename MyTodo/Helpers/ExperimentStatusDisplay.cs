using MyTodo.Domain.Enums;

namespace MyTodo.Helpers
{
    public static class ExperimentStatusDisplay
    {
        public static string GetBadgeClass(ExperimentStatus status) => status switch
        {
            ExperimentStatus.Innovation => "bg-secondary",
            ExperimentStatus.Verifying => "bg-info text-dark",
            ExperimentStatus.Verified => "bg-primary",
            ExperimentStatus.AddedInSOP => "bg-warning text-dark",
            ExperimentStatus.Discarded => "bg-dark",
            _ => "bg-secondary"
        };

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
