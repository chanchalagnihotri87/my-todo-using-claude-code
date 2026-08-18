using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface ISprintRepository : IBaseRepository<Sprint>
    {
        Task<Sprint?> GetCurrentAsync(DateTime today);
    }
}
