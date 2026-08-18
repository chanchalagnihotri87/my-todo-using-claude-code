using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface ISolutionRepository : IBaseRepository<Solution>
    {
        Task<List<Solution>> GetByProblemIdAsync(int problemId);
    }
}
