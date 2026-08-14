using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class TodoTaskService : ITodoTaskService
    {
        private readonly ITodoTaskRepository _todoTaskRepository;

        public TodoTaskService(ITodoTaskRepository todoTaskRepository)
        {
            _todoTaskRepository = todoTaskRepository;
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

            return MapToDto(task);
        }

        public async Task<TodoTaskDto?> UpdateAsync(UpdateTodoTaskDto updateTodoTaskDto)
        {
            var task = await _todoTaskRepository.GetByIdAsync(updateTodoTaskDto.Id);
            if (task == null)
            {
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
                return null;
            }

            SetStatus(task, status);
            task.UpdatedAt = DateTime.UtcNow;
            await _todoTaskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        private static void SetStatus(TodoTask task, TodoStatus status)
        {
            if (task.Status != status)
            {
                task.CompletedAt = status == TodoStatus.Completed ? DateTime.UtcNow : null;
            }

            task.Status = status;
        }

        public async Task<bool> UpdateSprintAsync(int id, int? sprintId)
        {
            var task = await _todoTaskRepository.GetByIdAsync(id);
            if (task == null)
            {
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
                return false;
            }

            await _todoTaskRepository.DeleteAsync(task);

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
