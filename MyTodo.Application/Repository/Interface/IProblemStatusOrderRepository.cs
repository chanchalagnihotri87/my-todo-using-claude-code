using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Repository.Interface
{
    public interface IProblemStatusOrderRepository : IBaseRepository<ProblemStatusOrder>
    {
        Task<List<ProblemStatusOrder>> GetAllOrderedAsync();
        Task ReorderAsync(List<ProblemStatus> orderedStatuses);
    }
}
