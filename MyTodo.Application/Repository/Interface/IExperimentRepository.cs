using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repository.Interface
{
    public interface IExperimentRepository : IBaseRepository<Experiment>
    {
        Task<List<Experiment>> GetBySolutionIdAsync(int solutionId);
    }
}
