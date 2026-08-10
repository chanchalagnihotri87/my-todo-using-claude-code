using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface IProblemStatusOrderService
    {
        Task<List<ProblemStatus>> GetOrderAsync();
        Task ReorderAsync(List<ProblemStatus> orderedStatuses);
    }
}
