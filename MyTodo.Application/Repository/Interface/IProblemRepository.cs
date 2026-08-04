using MyTodo.Domain.Entities;

namespace MyTodo.Application.Repository.Interface
{
    public interface IProblemRepository : IBaseRepository<Problem>
    {
        Task<List<Problem>> GetByLifeAreaIdAsync(int lifeAreaId);
    }
}
