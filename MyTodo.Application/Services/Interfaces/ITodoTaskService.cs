using MyTodo.Application.DTOs;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Services.Interfaces
{
    public interface ITodoTaskService
    {
        Task<List<TodoTaskDto>> GetByObjectiveIdAsync(int objectiveId);
        Task<List<TodoTaskDto>> GetBySprintIdAsync(int sprintId);
        Task<TodoTaskDto> CreateAsync(CreateTodoTaskDto createTodoTaskDto);
        Task<TodoTaskDto?> UpdateAsync(UpdateTodoTaskDto updateTodoTaskDto);
        Task<TodoTaskDto?> UpdateStatusAsync(int id, TodoStatus status);
        Task<bool> UpdateSprintAsync(int id, int? sprintId);
        Task<bool> DeleteAsync(int id);
    }
}
