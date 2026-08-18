using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repositories.Interfaces
{
    public interface IProblemRepository : IBaseRepository<Problem>
    {
        Task<List<Problem>> GetByLifeAreaIdAsync(int lifeAreaId);
    }
}
