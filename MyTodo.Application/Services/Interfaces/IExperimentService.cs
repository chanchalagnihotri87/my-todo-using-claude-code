using MyTodo.Application.DTOs;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface IExperimentService
    {
        Task<List<ExperimentDto>> GetBySolutionIdAsync(int solutionId);
        Task<ExperimentDto> CreateAsync(CreateExperimentDto createExperimentDto);
        Task<ExperimentDto?> UpdateAsync(UpdateExperimentDto updateExperimentDto);
        Task<bool> ReorderAsync(int id, ExperimentStatus status, List<int> orderedIds);
        Task<bool> DeleteAsync(int id);
    }
}
