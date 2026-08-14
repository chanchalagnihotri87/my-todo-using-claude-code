using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repository.Interface
{
    public interface ISprintRepository : IBaseRepository<Sprint>
    {
        Task<Sprint?> GetCurrentAsync(DateTime today);
    }
}
