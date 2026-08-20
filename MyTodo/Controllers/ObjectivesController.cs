using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Shared.Enums;
using MyTodo.Extensions;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ObjectivesController : Controller
    {
        private readonly IObjectiveService _objectiveService;
        private readonly ISolutionService _solutionService;
        private readonly ILogger<ObjectivesController> _logger;

        public ObjectivesController(IObjectiveService objectiveService, ISolutionService solutionService, ILogger<ObjectivesController> logger)
        {
            _objectiveService = objectiveService;
            _solutionService = solutionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int solutionId)
        {
            var solution = await _solutionService.GetByIdAsync(solutionId);
            if (solution == null)
            {
                _logger.LogWarning("Solution {SolutionId} not found when loading objectives index", solutionId);
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
                _logger.LogWarning("Invalid model state when creating objective for solution {SolutionId}", model?.SolutionId);
                return BadRequest();
            }

            var createObjectiveDto = new CreateObjectiveDto
            {
                SolutionId = model.SolutionId,
                Text = model.Text
            };

            await _objectiveService.CreateAsync(createObjectiveDto);
            _logger.LogInformation("Created objective for solution {SolutionId}", model.SolutionId);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateObjectiveRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when editing objective {ObjectiveId}", request.Id);
                return BadRequest();
            }

            if (!_logger.TryParseOrLogWarning<ObjectiveStatus>(request.Status, $"editing objective {request.Id}", out var status))
            {
                return BadRequest();
            }

            var updateObjectiveDto = new UpdateObjectiveDto
            {
                Id = request.Id,
                Text = request.Text,
                Status = status
            };

            var updated = await _objectiveService.UpdateAsync(updateObjectiveDto);
            if (updated == null)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found when editing", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated objective {ObjectiveId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderObjectivesRequest request)
        {
            if (!_logger.TryParseOrLogWarning<ObjectiveStatus>(request.Status, $"reordering objective {request.Id}", out var status))
            {
                return BadRequest();
            }

            var updated = await _objectiveService.ReorderAsync(request.Id, status, request.OrderedIds);
            if (!updated)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found when reordering", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Reordered objective {ObjectiveId} to status {Status}", request.Id, status);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderFocus([FromBody] ReorderObjectivesFocusRequest request)
        {
            var updated = await _objectiveService.ReorderFocusAsync(request.Id, request.IsTwentyPercent, request.OrderedIds);
            if (!updated)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found when reordering focus", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Reordered focus for objective {ObjectiveId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteObjectiveRequest request)
        {
            var deleted = await _objectiveService.DeleteAsync(request.Id);
            if (!deleted)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found when deleting", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Deleted objective {ObjectiveId}", request.Id);
            return Ok();
        }
    }
}
