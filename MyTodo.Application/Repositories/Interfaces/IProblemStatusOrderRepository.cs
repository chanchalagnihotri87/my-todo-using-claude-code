using MyTodo.Domain.Entities;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface IProblemStatusOrderRepository : IBaseRepository<ProblemStatusOrder>
    {
        Task<List<ProblemStatusOrder>> GetAllOrderedAsync();
        Task ReorderAsync(List<ProblemStatus> orderedStatuses);
    }
}
