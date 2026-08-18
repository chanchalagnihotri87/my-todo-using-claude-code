using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface ITodoTaskRepository : IBaseRepository<TodoTask>
    {
        Task<List<TodoTask>> GetByObjectiveIdAsync(int objectiveId);
        Task<List<TodoTask>> GetBySprintIdAsync(int sprintId);
    }
}
