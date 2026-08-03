using MyTodo.Application.DTOs;

namespace MyTodo.Application.Services.Interfaces
{
    public interface ILifeAreaService
    {
        Task<List<LifeAreaDto>> GetAllAsync();
    }
}
