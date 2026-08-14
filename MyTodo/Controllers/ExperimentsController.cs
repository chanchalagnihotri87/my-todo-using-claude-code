using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ExperimentsController : Controller
    {
        private readonly IExperimentService _experimentService;
        private readonly ISolutionService _solutionService;

        public ExperimentsController(IExperimentService experimentService, ISolutionService solutionService)
        {
            _experimentService = experimentService;
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

            var experiments = await _experimentService.GetBySolutionIdAsync(solutionId);
            return View(experiments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateExperimentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var createExperimentDto = new CreateExperimentDto
            {
                SolutionId = model.SolutionId,
                Name = model.Name,
                Description = model.Description
            };

            await _experimentService.CreateAsync(createExperimentDto);

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateExperimentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            {
                return BadRequest();
            }

            if (!Enum.TryParse<ExperimentStatus>(request.Status, out var status))
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
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderExperimentsRequest request)
        {
            if (!Enum.TryParse<ExperimentStatus>(request.Status, out var status))
            {
                return BadRequest();
            }

            var updated = await _experimentService.ReorderAsync(request.Id, status, request.OrderedIds);
            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteExperimentRequest request)
        {
            var deleted = await _experimentService.DeleteAsync(request.Id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
