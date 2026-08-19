using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface IExperimentRepository : IBaseRepository<Experiment>
    {
        Task<List<Experiment>> GetBySolutionIdAsync(int solutionId);
        Task<int> CountBySolutionIdAsync(int solutionId);
    }
}
