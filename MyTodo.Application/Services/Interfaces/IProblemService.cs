using MyTodo.Application.DTOs;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface IProblemService
    {
        Task<List<ProblemDto>> GetByLifeAreaIdAsync(int lifeAreaId);
        Task<ProblemDto> CreateAsync(CreateProblemDto createProblemDto);
        Task<ProblemDto?> UpdateAsync(UpdateProblemDto updateProblemDto);
        Task<ProblemDto?> UpdateStatusAsync(int id, ProblemStatus status);
        Task<ProblemDto?> ToggleUrgentAsync(int id);
        Task<ProblemDto?> ToggleImportantAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
