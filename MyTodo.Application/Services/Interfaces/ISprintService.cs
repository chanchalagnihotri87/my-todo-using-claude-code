using MyTodo.Application.DTOs;

namespace MyTodo.Application.Services.Interfaces
{
    public interface ISprintService
    {
        Task<List<SprintDto>> GetAllAsync();
        Task<SprintDto?> GetByIdAsync(int id);
        Task<SprintDto?> GetCurrentAsync();
        Task<SprintDto> CreateAsync(CreateSprintDto createSprintDto);
        Task<SprintDto?> UpdateAsync(UpdateSprintDto updateSprintDto);
        Task<bool> DeleteAsync(int id);
    }
}
