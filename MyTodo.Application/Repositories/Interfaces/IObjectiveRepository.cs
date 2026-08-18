using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface IObjectiveRepository : IBaseRepository<Objective>
    {
        Task<List<Objective>> GetBySolutionIdAsync(int solutionId);
    }
}
