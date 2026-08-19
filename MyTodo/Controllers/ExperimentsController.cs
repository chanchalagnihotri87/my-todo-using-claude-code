using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Extensions;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ExperimentsController : Controller
    {
        private readonly IExperimentService _experimentService;
        private readonly ISolutionService _solutionService;
        private readonly ILogger<ExperimentsController> _logger;

        public ExperimentsController(IExperimentService experimentService, ISolutionService solutionService, ILogger<ExperimentsController> logger)
        {
            _experimentService = experimentService;
            _solutionService = solutionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int solutionId)
        {
            var solution = await _solutionService.GetByIdAsync(solutionId);
            if (solution == null)
            {
                _logger.LogWarning("Solution {SolutionId} not found when loading experiments index", solutionId);
                return NotFound();
            }

            ViewBag.Solution = solution;

            var experiments = await _experimentService.GetBySolutionIdAsync(solutionId);
            return View(experiments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateExperimentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when creating experiment for solution {SolutionId}", model?.SolutionId);
                return BadRequest();
            }

            var createExperimentDto = new CreateExperimentDto
            {
                SolutionId = model.SolutionId,
                Name = model.Name,
                Description = model.Description
            };

            await _experimentService.CreateAsync(createExperimentDto);
            _logger.LogInformation("Created experiment {ExperimentName} for solution {SolutionId}", model.Name, model.SolutionId);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateExperimentRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when editing experiment {ExperimentId}", request.Id);
                return BadRequest();
            }

            if (!_logger.TryParseOrLogWarning<ExperimentStatus>(request.Status, $"editing experiment {request.Id}", out var status))
            {
                return BadRequest();
            }

            var updateExperimentDto = new UpdateExperimentDto
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Status = status
            };

            var updated = await _experimentService.UpdateAsync(updateExperimentDto);
            if (updated == null)
            {
                _logger.LogWarning("Experiment {ExperimentId} not found when editing", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated experiment {ExperimentId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderExperimentsRequest request)
        {
            if (!_logger.TryParseOrLogWarning<ExperimentStatus>(request.Status, $"reordering experiment {request.Id}", out var status))
            {
                return BadRequest();
            }

            var updated = await _experimentService.ReorderAsync(request.Id, status, request.OrderedIds);
            if (!updated)
            {
                _logger.LogWarning("Experiment {ExperimentId} not found when reordering", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Reordered experiment {ExperimentId} to status {Status}", request.Id, status);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteExperimentRequest request)
        {
            var deleted = await _experimentService.DeleteAsync(request.Id);
            if (!deleted)
            {
                _logger.LogWarning("Experiment {ExperimentId} not found when deleting", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Deleted experiment {ExperimentId}", request.Id);
            return Ok();
        }
    }
}
