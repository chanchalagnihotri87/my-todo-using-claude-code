using MyTodo.Application.DTOs;

namespace MyTodo.Application.Services.Interfaces
{
    public interface IProblemService
    {
        Task<List<ProblemDto>> GetByLifeAreaIdAsync(int lifeAreaId);
    }
}
