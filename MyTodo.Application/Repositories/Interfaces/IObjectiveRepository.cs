using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface IObjectiveRepository : IBaseRepository<Objective>
    {
        Task<List<Objective>> GetBySolutionIdAsync(int solutionId);
        Task<int> CountBySolutionIdAsync(int solutionId);
        Task<Dictionary<int, (int Total, int Completed)>> GetObjectiveCountsBySolutionIdsAsync(IEnumerable<int> solutionIds);
    }
}
