using MyTodo.Application.DTOs;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface ISolutionService
    {
        Task<List<SolutionDto>> GetByProblemIdAsync(int problemId);
        Task<SolutionDto?> GetByIdAsync(int id);
        Task<SolutionDto> CreateAsync(CreateSolutionDto createSolutionDto);
        Task<bool> ReorderAsync(int id, SolutionStatus status, List<int> orderedIds);
        Task<bool> ReorderTwentyPercentAsync(int id, bool isTwentyPercent, List<int> orderedIds);
    }
}
