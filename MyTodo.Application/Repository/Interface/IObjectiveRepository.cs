using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repository.Interface
{
    public interface IObjectiveRepository : IBaseRepository<Objective>
    {
        Task<List<Objective>> GetBySolutionIdAsync(int solutionId);
    }
}
