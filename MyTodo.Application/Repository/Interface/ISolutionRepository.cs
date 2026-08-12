using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repository.Interface
{
    public interface ISolutionRepository : IBaseRepository<Solution>
    {
        Task<List<Solution>> GetByProblemIdAsync(int problemId);
    }
}
