using MyTodo.Application.DTOs;

namespace MyTodo.Application.Services.Interfaces
{
    public interface ITodoService
    {
        Task<TodoDto> AddToTodoAsync(int todoTaskId);
        Task<TodoDto?> UpdateDateAsync(int todoId, DateOnly todoDate);
        Task<TodoDto?> ToggleUrgentAsync(int todoId);
        Task<TodoDto?> ToggleImportantAsync(int todoId);
        Task<TodoDto?> ToggleFrogAsync(int todoId);
        Task<List<TodoDto>> GetTodayAsync();
        Task<List<TodoDto>> GetHistoryAsync(int? objectiveId, DateOnly? fromDate, DateOnly? toDate);
    }
}
