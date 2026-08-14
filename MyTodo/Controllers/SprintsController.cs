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

        public SprintsController(ISprintService sprintService, ITodoTaskService todoTaskService)
        {
            _sprintService = sprintService;
            _todoTaskService = todoTaskService;
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

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateSprintRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            {
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
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteSprintRequest request)
        {
            var deleted = await _sprintService.DeleteAsync(request.Id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
