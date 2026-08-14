using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<TodoDto> AddToTodoAsync(int todoTaskId)
        {
            var existing = await _todoRepository.GetByTodoTaskIdAsync(todoTaskId);
            if (existing != null)
            {
                return MapToDto(existing);
            }

            var todo = new Todo
            {
                TodoTaskId = todoTaskId,
                TodoDate = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow
            };

            await _todoRepository.AddAsync(todo);

            return MapToDto(todo);
        }

        public async Task<TodoDto?> UpdateDateAsync(int todoId, DateOnly todoDate)
        {
            var todo = await _todoRepository.GetByIdAsync(todoId);
            if (todo == null)
            {
                return null;
            }

            todo.TodoDate = todoDate;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateAsync(todo);

            return MapToDto(todo);
        }

        public async Task<TodoDto?> ToggleUrgentAsync(int todoId)
        {
            var todo = await _todoRepository.GetByIdAsync(todoId);
            if (todo == null)
            {
                return null;
            }

            todo.IsUrgent = !todo.IsUrgent;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateAsync(todo);

            return MapToDto(todo);
        }

        public async Task<TodoDto?> ToggleImportantAsync(int todoId)
        {
            var todo = await _todoRepository.GetByIdAsync(todoId);
            if (todo == null)
            {
                return null;
            }

            todo.IsImportant = !todo.IsImportant;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateAsync(todo);

            return MapToDto(todo);
        }

        public async Task<TodoDto?> ToggleFrogAsync(int todoId)
        {
            var todo = await _todoRepository.GetByIdAsync(todoId);
            if (todo == null)
            {
                return null;
            }

            var newValue = !todo.IsFrog;
            if (newValue)
            {
                var existingFrog = await _todoRepository.GetFrogByDateAsync(todo.TodoDate);
                if (existingFrog != null && existingFrog.Id != todo.Id)
                {
                    existingFrog.IsFrog = false;
                    existingFrog.UpdatedAt = DateTime.UtcNow;
                    await _todoRepository.UpdateAsync(existingFrog);
                }
            }

            todo.IsFrog = newValue;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateAsync(todo);

            return MapToDto(todo);
        }

        public async Task<List<TodoDto>> GetTodayAsync()
        {
            var todos = await _todoRepository.GetByDateAsync(DateOnly.FromDateTime(DateTime.UtcNow));
            return todos.Select(MapToDto).ToList();
        }

        public async Task<List<TodoDto>> GetHistoryAsync(int? objectiveId, DateOnly? fromDate, DateOnly? toDate)
        {
            var todos = await _todoRepository.GetHistoryAsync(objectiveId, fromDate, toDate);
            return todos.Select(MapToDto).ToList();
        }

        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                TodoTaskId = todo.TodoTaskId,
                TodoTaskName = todo.TodoTask?.Name ?? string.Empty,
                TaskStatus = todo.TodoTask?.Status ?? TodoStatus.Pending,
                ObjectiveId = todo.TodoTask?.ObjectiveId ?? 0,
                ObjectiveText = todo.TodoTask?.Objective?.Text ?? string.Empty,
                SprintName = todo.TodoTask?.Sprint?.Name,
                TodoDate = todo.TodoDate,
                IsUrgent = todo.IsUrgent,
                IsImportant = todo.IsImportant,
                IsFrog = todo.IsFrog,
                CompletedAt = todo.TodoTask?.CompletedAt,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            };
        }
    }
}
