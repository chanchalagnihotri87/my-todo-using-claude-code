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

        public TasksController(ITodoTaskService todoTaskService, IObjectiveService objectiveService, ISprintService sprintService, ITodoService todoService)
        {
            _todoTaskService = todoTaskService;
            _objectiveService = objectiveService;
            _sprintService = sprintService;
            _todoService = todoService;
        }

        public async Task<IActionResult> Index(int objectiveId)
        {
            var objective = await _objectiveService.GetByIdAsync(objectiveId);
            if (objective == null)
            {
                return NotFound();
            }

            ViewBag.Objective = objective;
            ViewBag.Sprints = await _sprintService.GetAllAsync();

            var tasks = await _todoTaskService.GetByObjectiveIdAsync(objectiveId);
            return View(tasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateTodoTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createTodoTaskDto = new CreateTodoTaskDto
            {
                ObjectiveId = model.ObjectiveId,
                Name = model.Name
            };

            await _todoTaskService.CreateAsync(createTodoTaskDto);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateTodoTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            {
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
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateTodoTaskStatusRequest request)
        {
            var updated = await _todoTaskService.UpdateStatusAsync(request.Id, request.Status);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSprint([FromBody] UpdateTaskSprintRequest request)
        {
            var updated = await _todoTaskService.UpdateSprintAsync(request.Id, request.SprintId);
            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToTodo([FromBody] AddToTodoRequest request)
        {
            var todo = await _todoService.AddToTodoAsync(request.Id);
            return Ok(todo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTodoDate([FromBody] UpdateTodoDateRequest request)
        {
            var updated = await _todoService.UpdateDateAsync(request.Id, request.TodoDate);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteTodoTaskRequest request)
        {
            var deleted = await _todoTaskService.DeleteAsync(request.Id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
