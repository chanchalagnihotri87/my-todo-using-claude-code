using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ObjectivesController : Controller
    {
        private readonly IObjectiveService _objectiveService;
        private readonly ISolutionService _solutionService;

        public ObjectivesController(IObjectiveService objectiveService, ISolutionService solutionService)
        {
            _objectiveService = objectiveService;
            _solutionService = solutionService;
        }

        public async Task<IActionResult> Index(int solutionId)
        {
            var solution = await _solutionService.GetByIdAsync(solutionId);
            if (solution == null)
            {
                return NotFound();
            }

            ViewBag.Solution = solution;

            var objectives = await _objectiveService.GetBySolutionIdAsync(solutionId);
            return View(objectives);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateObjectiveViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createObjectiveDto = new CreateObjectiveDto
            {
                SolutionId = model.SolutionId,
                Text = model.Text
            };

            await _objectiveService.CreateAsync(createObjectiveDto);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateObjectiveStatusRequest request)
        {
            if (!Enum.TryParse<ObjectiveStatus>(request.Status, out var status))
            {
                return BadRequest();
            }

            var updated = await _objectiveService.UpdateStatusAsync(request.Id, status);
            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderObjectivesRequest request)
        {
            if (!Enum.TryParse<ObjectiveStatus>(request.Status, out var status))
            {
                return BadRequest();
            }

            var updated = await _objectiveService.ReorderAsync(request.Id, status, request.OrderedIds);
            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteObjectiveRequest request)
        {
            var deleted = await _objectiveService.DeleteAsync(request.Id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
