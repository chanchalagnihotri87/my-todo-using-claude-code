using Microsoft.Extensions.Logging;
using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Common;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ILogger<TodoService> _logger;

        public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }

        public async Task<TodoDto> AddToTodoAsync(int todoTaskId)
        {
            var existing = await _todoRepository.GetByTodoTaskIdAsync(todoTaskId);
            if (existing != null)
            {
                return MapToDto(existing);
            }

            var todoDate = DateOnly.FromDateTime(DateTime.UtcNow);
            var maxSortOrder = await _todoRepository.GetMaxSortOrderAsync(todoDate);

            var todo = new Todo
            {
                TodoTaskId = todoTaskId,
                TodoDate = todoDate,
                SortOrder = maxSortOrder + 1,
                CreatedAt = DateTime.UtcNow
            };

            await _todoRepository.AddAsync(todo);

            _logger.LogInformation("Todo {TodoId} created for task {TodoTaskId}", todo.Id, todoTaskId);

            return MapToDto(todo);
        }

        public async Task<TodoDto?> UpdateDateAsync(int todoId, DateOnly todoDate)
        {
            var todo = await _todoRepository.GetByIdAsync(todoId);
            if (todo == null)
            {
                _logger.LogWarning("Todo {TodoId} not found for date update", todoId);
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
                _logger.LogWarning("Todo {TodoId} not found for urgent toggle", todoId);
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
                _logger.LogWarning("Todo {TodoId} not found for important toggle", todoId);
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
                _logger.LogWarning("Todo {TodoId} not found for frog toggle", todoId);
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

        public async Task ReorderAsync(List<int> orderedTodoIds)
        {
            await ReorderHelper.ReindexAsync(
                _todoRepository,
                x => x.Id,
                orderedTodoIds,
                (todo, index) =>
                {
                    todo.SortOrder = index;
                    todo.UpdatedAt = DateTime.UtcNow;
                });
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
                SortOrder = todo.SortOrder,
                CompletedAt = todo.TodoTask?.CompletedAt,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            };
        }
    }
}
