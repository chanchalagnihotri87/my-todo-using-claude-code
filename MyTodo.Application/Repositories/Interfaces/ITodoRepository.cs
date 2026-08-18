using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface ITodoRepository : IBaseRepository<Todo>
    {
        Task<Todo?> GetByTodoTaskIdAsync(int todoTaskId);
        Task<List<Todo>> GetByDateAsync(DateOnly date);
        Task<Todo?> GetFrogByDateAsync(DateOnly date);
        Task<List<Todo>> GetHistoryAsync(int? objectiveId, DateOnly? fromDate, DateOnly? toDate);
        Task<int> GetMaxSortOrderAsync(DateOnly date);
    }
}
