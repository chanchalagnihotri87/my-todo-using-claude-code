using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly ISolutionService _solutionService;
        private readonly IProblemService _problemService;

        public SolutionsController(ISolutionService solutionService, IProblemService problemService)
        {
            _solutionService = solutionService;
            _problemService = problemService;
        }

        public async Task<IActionResult> Index(int problemId)
        {
            var problem = await _problemService.GetByIdAsync(problemId);
            if (problem == null)
            {
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
                return NotFound();
            }

            return View(solution);
        }

        public async Task<IActionResult> Create(int problemId)
        {
            var problem = await _problemService.GetByIdAsync(problemId);
            if (problem == null)
            {
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

            return RedirectToAction(nameof(Index), new { problemId = model.ProblemId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromBody] ReorderSolutionsRequest request)
        {
            if (!Enum.TryParse<SolutionStatus>(request.Status, out var status))
            {
                return BadRequest();
            }

            var updated = await _solutionService.ReorderAsync(request.Id, status, request.OrderedIds);
            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderFocus([FromBody] ReorderFocusRequest request)
        {
            var updated = await _solutionService.ReorderTwentyPercentAsync(request.Id, request.IsTwentyPercent, request.OrderedIds);
            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
