using MyTodo.Application.DTOs;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface IObjectiveService
    {
        Task<List<ObjectiveDto>> GetBySolutionIdAsync(int solutionId);
        Task<List<ObjectiveDto>> GetAllAsync();
        Task<ObjectiveDto?> GetByIdAsync(int id);
        Task<ObjectiveDto> CreateAsync(CreateObjectiveDto createObjectiveDto);
        Task<ObjectiveDto?> UpdateAsync(UpdateObjectiveDto updateObjectiveDto);
        Task<bool> ReorderAsync(int id, ObjectiveStatus status, List<int> orderedIds);
        Task<bool> ReorderFocusAsync(int id, bool isTwentyPercent, List<int> orderedIds);
        Task<bool> DeleteAsync(int id);
    }
}
