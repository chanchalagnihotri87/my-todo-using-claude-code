using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class SprintsController : Controller
    {
        private readonly ISprintService _sprintService;
        private readonly ITodoTaskService _todoTaskService;
        private readonly ILogger<SprintsController> _logger;

        public SprintsController(ISprintService sprintService, ITodoTaskService todoTaskService, ILogger<SprintsController> logger)
        {
            _sprintService = sprintService;
            _todoTaskService = todoTaskService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var current = await _sprintService.GetCurrentAsync();
            if (current == null)
            {
                ViewBag.Sprint = null;
                return View(new List<TodoTaskDto>());
            }

            var tasks = await _todoTaskService.GetBySprintIdAsync(current.Id);
            ViewBag.Sprint = current;
            return View(tasks);
        }

        public async Task<IActionResult> All()
        {
            var sprints = await _sprintService.GetAllAsync();
            return View(sprints);
        }

        public async Task<IActionResult> Details(int id)
        {
            var sprint = await _sprintService.GetByIdAsync(id);
            if (sprint == null)
            {
                _logger.LogWarning("Sprint {SprintId} not found when loading details", id);
                return NotFound();
            }

            var tasks = await _todoTaskService.GetBySprintIdAsync(id);
            ViewBag.Sprint = sprint;
            return View(tasks);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateSprintViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating sprint");
                return BadRequest();
            }

            var createSprintDto = new CreateSprintDto
            {
                Name = model.Name,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            await _sprintService.CreateAsync(createSprintDto);
            _logger.LogInformation("Created sprint {SprintName}", model.Name);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateSprintRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when editing sprint {SprintId}", request.Id);
                return BadRequest();
            }

            var updateSprintDto = new UpdateSprintDto
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var updated = await _sprintService.UpdateAsync(updateSprintDto);
            if (updated == null)
            {
                _logger.LogWarning("Sprint {SprintId} not found when editing", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated sprint {SprintId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteSprintRequest request)
        {
            var deleted = await _sprintService.DeleteAsync(request.Id);
            if (!deleted)
            {
                _logger.LogWarning("Sprint {SprintId} not found when deleting", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Deleted sprint {SprintId}", request.Id);
            return Ok();
        }
    }
}
