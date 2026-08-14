using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITodoTaskService _todoTaskService;
        private readonly IObjectiveService _objectiveService;
        private readonly ISprintService _sprintService;
        private readonly ITodoService _todoService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITodoTaskService todoTaskService, IObjectiveService objectiveService, ISprintService sprintService, ITodoService todoService, ILogger<TasksController> logger)
        {
            _todoTaskService = todoTaskService;
            _objectiveService = objectiveService;
            _sprintService = sprintService;
            _todoService = todoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int objectiveId)
        {
            var objective = await _objectiveService.GetByIdAsync(objectiveId);
            if (objective == null)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found when loading tasks index", objectiveId);
                return NotFound();
            }

            ViewBag.Objective = objective;
            ViewBag.Sprints = await _sprintService.GetAllAsync();

            var tasks = await _todoTaskService.GetByObjectiveIdAsync(objectiveId);
            _logger.LogInformation("Loaded {TaskCount} tasks for objective {ObjectiveId}", tasks.Count(), objectiveId);
            return View(tasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateTodoTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating task for objective {ObjectiveId}", model?.ObjectiveId);
                return BadRequest();
            }

            var createTodoTaskDto = new CreateTodoTaskDto
            {
                ObjectiveId = model.ObjectiveId,
                Name = model.Name
            };

            await _todoTaskService.CreateAsync(createTodoTaskDto);
            _logger.LogInformation("Created task {TaskName} for objective {ObjectiveId}", model.Name, model.ObjectiveId);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateTodoTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            {
                _logger.LogWarning("Invalid name when editing task {TaskId}", request.Id);
                return BadRequest();
            }

            var updateTodoTaskDto = new UpdateTodoTaskDto
            {
                Id = request.Id,
                Name = request.Name,
                Status = request.Status,
                SprintId = request.SprintId
            };

            var updated = await _todoTaskService.UpdateAsync(updateTodoTaskDto);
            if (updated == null)
            {
                _logger.LogWarning("Task {TaskId} not found when editing", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated task {TaskId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTodoTaskStatusRequest request)
        {
            var updated = await _todoTaskService.UpdateStatusAsync(request.Id, request.Status);
            if (updated == null)
            {
                _logger.LogWarning("Task {TaskId} not found when updating status to {Status}", request.Id, request.Status);
                return NotFound();
            }

            _logger.LogInformation("Updated task {TaskId} status to {Status}", request.Id, request.Status);
            return Ok(updated);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSprint([FromBody] UpdateTaskSprintRequest request)
        {
            var updated = await _todoTaskService.UpdateSprintAsync(request.Id, request.SprintId);
            if (!updated)
            {
                _logger.LogWarning("Task {TaskId} not found when updating sprint to {SprintId}", request.Id, request.SprintId);
                return NotFound();
            }

            _logger.LogInformation("Updated task {TaskId} sprint to {SprintId}", request.Id, request.SprintId);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToTodo([FromBody] AddToTodoRequest request)
        {
            var todo = await _todoService.AddToTodoAsync(request.Id);
            _logger.LogInformation("Added task {TaskId} to todo", request.Id);
            return Ok(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTodoDate([FromBody] UpdateTodoDateRequest request)
        {
            var updated = await _todoService.UpdateDateAsync(request.Id, request.TodoDate);
            if (updated == null)
            {
                _logger.LogWarning("Todo {TodoId} not found when updating date to {TodoDate}", request.Id, request.TodoDate);
                return NotFound();
            }

            _logger.LogInformation("Updated todo {TodoId} date to {TodoDate}", request.Id, request.TodoDate);
            return Ok(updated);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteTodoTaskRequest request)
        {
            var deleted = await _todoTaskService.DeleteAsync(request.Id);
            if (!deleted)
            {
                _logger.LogWarning("Task {TaskId} not found when deleting", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Deleted task {TaskId}", request.Id);
            return Ok();
        }
    }
}
