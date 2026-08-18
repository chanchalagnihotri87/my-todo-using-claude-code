using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Extensions;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly ISolutionService _solutionService;
        private readonly IProblemService _problemService;
        private readonly ILogger<SolutionsController> _logger;

        public SolutionsController(ISolutionService solutionService, IProblemService problemService, ILogger<SolutionsController> logger)
        {
            _solutionService = solutionService;
            _problemService = problemService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int problemId)
        {
            var problem = await _problemService.GetByIdAsync(problemId);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when loading solutions index", problemId);
                return NotFound();
            }

            ViewBag.Problem = problem;

            var solutions = await _solutionService.GetByProblemIdAsync(problemId);
            return View(solutions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var solution = await _solutionService.GetByIdAsync(id);
            if (solution == null)
            {
                _logger.LogWarning("Solution {SolutionId} not found when loading details", id);
                return NotFound();
            }

            return View(solution);
        }

        public async Task<IActionResult> Create(int problemId)
        {
            var problem = await _problemService.GetByIdAsync(problemId);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when loading create solution page", problemId);
                return NotFound();
            }

            ViewBag.Problem = problem;

            return View(new CreateSolutionViewModel { ProblemId = problemId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSolutionViewModel model)
        {
            var problem = await _problemService.GetByIdAsync(model.ProblemId);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when creating solution", model.ProblemId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Problem = problem;
                return View(model);
            }

            var createSolutionDto = new CreateSolutionDto
            {
                ProblemId = model.ProblemId,
                Name = model.Name,
                Description = model.Description,
                IsTwentyPercent = model.IsTwentyPercent
            };

            await _solutionService.CreateAsync(createSolutionDto);
            _logger.LogInformation("Created solution {SolutionName} for problem {ProblemId}", model.Name, model.ProblemId);

            return RedirectToAction(nameof(Index), new { problemId = model.ProblemId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderSolutionsRequest request)
        {
            if (!_logger.TryParseOrLogWarning<SolutionStatus>(request.Status, $"reordering solution {request.Id}", out var status))
            {
                return BadRequest();
            }

            var updated = await _solutionService.ReorderAsync(request.Id, status, request.OrderedIds);
            if (!updated)
            {
                _logger.LogWarning("Solution {SolutionId} not found when reordering", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Reordered solution {SolutionId} to status {Status}", request.Id, status);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderFocus([FromBody] ReorderFocusRequest request)
        {
            var updated = await _solutionService.ReorderTwentyPercentAsync(request.Id, request.IsTwentyPercent, request.OrderedIds);
            if (!updated)
            {
                _logger.LogWarning("Solution {SolutionId} not found when reordering focus", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Reordered focus for solution {SolutionId}", request.Id);
            return Ok();
        }
    }
}
