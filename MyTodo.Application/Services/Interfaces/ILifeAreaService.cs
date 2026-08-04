using MyTodo.Application.DTOs;

namespace MyTodo.Application.Services.Interfaces
{
    public interface ILifeAreaService
    {
        Task<List<LifeAreaDto>> GetAllAsync();
        Task<LifeAreaDto?> GetByIdAsync(int id);
        Task<LifeAreaDto> CreateAsync(CreateLifeAreaDto createLifeAreaDto);
        Task<LifeAreaDto?> UpdateAsync(UpdateLifeAreaDto updateLifeAreaDto);
        Task<bool> DeleteAsync(int id);
    }
}
