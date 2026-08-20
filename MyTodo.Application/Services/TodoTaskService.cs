using Microsoft.Extensions.Logging;
using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class TodoTaskService : ITodoTaskService
    {
        private readonly ITodoTaskRepository _todoTaskRepository;
        private readonly ILogger<TodoTaskService> _logger;

        public TodoTaskService(ITodoTaskRepository todoTaskRepository, ILogger<TodoTaskService> logger)
        {
            _todoTaskRepository = todoTaskRepository;
            _logger = logger;
        }

        public async Task<List<TodoTaskDto>> GetByObjectiveIdAsync(int objectiveId)
        {
            var tasks = await _todoTaskRepository.GetByObjectiveIdAsync(objectiveId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<List<TodoTaskDto>> GetBySprintIdAsync(int sprintId)
        {
            var tasks = await _todoTaskRepository.GetBySprintIdAsync(sprintId);
            return tasks.Select(MapToDto).ToList();
        }

        public async Task<TodoTaskDto> CreateAsync(CreateTodoTaskDto createTodoTaskDto)
        {
            var task = new TodoTask
            {
                ObjectiveId = createTodoTaskDto.ObjectiveId,
                Name = createTodoTaskDto.Name,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _todoTaskRepository.AddAsync(task);

            _logger.LogInformation("TodoTask {TodoTaskId} created for objective {ObjectiveId}", task.Id, task.ObjectiveId);

            return MapToDto(task);
        }

        public async Task<TodoTaskDto?> UpdateAsync(UpdateTodoTaskDto updateTodoTaskDto)
        {
            var task = await _todoTaskRepository.GetByIdAsync(updateTodoTaskDto.Id);
            if (task == null)
            {
                _logger.LogWarning("TodoTask {TodoTaskId} not found for update", updateTodoTaskDto.Id);
                return null;
            }

            task.Name = updateTodoTaskDto.Name;
            SetStatus(task, updateTodoTaskDto.Status);
            task.SprintId = updateTodoTaskDto.SprintId;
            task.UpdatedAt = DateTime.UtcNow;
            await _todoTaskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        public async Task<TodoTaskDto?> UpdateStatusAsync(int id, TodoStatus status)
        {
            var task = await _todoTaskRepository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("TodoTask {TodoTaskId} not found for status update", id);
                return null;
            }

            SetStatus(task, status);
            task.UpdatedAt = DateTime.UtcNow;
            await _todoTaskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        private void SetStatus(TodoTask task, TodoStatus status)
        {
            if (task.Status != status)
            {
                task.CompletedAt = status == TodoStatus.Completed ? DateTime.UtcNow : null;
                _logger.LogInformation("TodoTask {TodoTaskId} status changed from {OldStatus} to {NewStatus}", task.Id, task.Status, status);
            }

            task.Status = status;
        }

        public async Task<bool> UpdateSprintAsync(int id, int? sprintId)
        {
            var task = await _todoTaskRepository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("TodoTask {TodoTaskId} not found for sprint update", id);
                return false;
            }

            task.SprintId = sprintId;
            task.UpdatedAt = DateTime.UtcNow;
            await _todoTaskRepository.UpdateAsync(task);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _todoTaskRepository.GetByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning("TodoTask {TodoTaskId} not found for delete", id);
                return false;
            }

            await _todoTaskRepository.DeleteAsync(task);

            _logger.LogInformation("TodoTask {TodoTaskId} deleted", id);

            return true;
        }

        private static TodoTaskDto MapToDto(TodoTask task)
        {
            return new TodoTaskDto
            {
                Id = task.Id,
                ObjectiveId = task.ObjectiveId,
                Name = task.Name,
                Status = task.Status,
                CompletedAt = task.CompletedAt,
                SprintId = task.SprintId,
                SprintName = task.Sprint?.Name,
                ObjectiveText = task.Objective?.Text,
                TodoId = task.Todo?.Id,
                TodoDate = task.Todo?.TodoDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
