using MyTodo.Application.DTOs;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface IObjectiveService
    {
        Task<List<ObjectiveDto>> GetBySolutionIdAsync(int solutionId);
        Task<ObjectiveDto> CreateAsync(CreateObjectiveDto createObjectiveDto);
        Task<bool> UpdateStatusAsync(int id, ObjectiveStatus status);
        Task<bool> ReorderAsync(int id, ObjectiveStatus status, List<int> orderedIds);
        Task<bool> DeleteAsync(int id);
    }
}
