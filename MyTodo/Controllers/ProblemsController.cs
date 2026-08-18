using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Extensions;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ProblemsController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ILifeAreaService _lifeAreaService;
        private readonly IProblemStatusOrderService _problemStatusOrderService;
        private readonly ILogger<ProblemsController> _logger;

        public ProblemsController(IProblemService problemService, ILifeAreaService lifeAreaService, IProblemStatusOrderService problemStatusOrderService, ILogger<ProblemsController> logger)
        {
            _problemService = problemService;
            _lifeAreaService = lifeAreaService;
            _problemStatusOrderService = problemStatusOrderService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int lifeAreaId)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(lifeAreaId);
            if (lifeArea == null)
            {
                _logger.LogWarning("Life area {LifeAreaId} not found when loading problems index", lifeAreaId);
                return NotFound();
            }

            ViewBag.LifeArea = lifeArea;
            ViewBag.ColumnOrder = await _problemStatusOrderService.GetOrderAsync();

            var problems = await _problemService.GetByLifeAreaIdAsync(lifeAreaId);
            return View(problems);
        }

        public async Task<IActionResult> Create(int lifeAreaId)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(lifeAreaId);
            if (lifeArea == null)
            {
                _logger.LogWarning("Life area {LifeAreaId} not found when loading create problem page", lifeAreaId);
                return NotFound();
            }

            ViewBag.LifeArea = lifeArea;

            return View(new CreateProblemViewModel { LifeAreaId = lifeAreaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProblemViewModel model)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(model.LifeAreaId);
            if (lifeArea == null)
            {
                _logger.LogWarning("Life area {LifeAreaId} not found when creating problem", model.LifeAreaId);
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.LifeArea = lifeArea;
                return View(model);
            }

            var createProblemDto = new CreateProblemDto
            {
                LifeAreaId = model.LifeAreaId,
                Name = model.Name,
                Description = model.Description
            };

            await _problemService.CreateAsync(createProblemDto);
            _logger.LogInformation("Created problem {ProblemName} for life area {LifeAreaId}", model.Name, model.LifeAreaId);

            return RedirectToAction(nameof(Index), new { lifeAreaId = model.LifeAreaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateProblemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            {
                _logger.LogWarning("Invalid name when editing problem {ProblemId}", request.Id);
                return BadRequest();
            }

            if (!_logger.TryParseOrLogWarning<ProblemStatus>(request.Status, $"editing problem {request.Id}", out var status))
            {
                return BadRequest();
            }

            var updateProblemDto = new UpdateProblemDto
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Status = status,
                IsUrgent = request.IsUrgent,
                IsImportant = request.IsImportant
            };

            var updated = await _problemService.UpdateAsync(updateProblemDto);
            if (updated == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when editing", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated problem {ProblemId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteProblemRequest request)
        {
            var deleted = await _problemService.DeleteAsync(request.Id);
            if (!deleted)
            {
                _logger.LogWarning("Problem {ProblemId} not found when deleting", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Deleted problem {ProblemId}", request.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateProblemStatusRequest request)
        {
            if (!_logger.TryParseOrLogWarning<ProblemStatus>(request.Status, $"updating problem {request.Id} status", out var status))
            {
                return BadRequest();
            }

            var updated = await _problemService.UpdateStatusAsync(request.Id, status);
            if (updated == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when updating status to {Status}", request.Id, status);
                return NotFound();
            }

            _logger.LogInformation("Updated problem {ProblemId} status to {Status}", request.Id, status);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderLists([FromBody] ReorderListsRequest request)
        {
            var statuses = new List<ProblemStatus>();
            foreach (var value in request.OrderedStatuses)
            {
                if (!_logger.TryParseOrLogWarning<ProblemStatus>(value, "reordering problem lists", out var status))
                {
                    return BadRequest();
                }
                statuses.Add(status);
            }

            await _problemStatusOrderService.ReorderAsync(statuses);
            _logger.LogInformation("Reordered problem status lists");
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUrgent([FromBody] ToggleProblemTagRequest request)
        {
            var updated = await _problemService.ToggleUrgentAsync(request.Id);
            if (updated == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when toggling urgent", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Toggled urgent to {IsUrgent} for problem {ProblemId}", updated.IsUrgent, request.Id);
            return Json(new { isUrgent = updated.IsUrgent });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleImportant([FromBody] ToggleProblemTagRequest request)
        {
            var updated = await _problemService.ToggleImportantAsync(request.Id);
            if (updated == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found when toggling important", request.Id);
                return NotFound();
            }

            _logger.LogInformation("Toggled important to {IsImportant} for problem {ProblemId}", updated.IsImportant, request.Id);
            return Json(new { isImportant = updated.IsImportant });
        }
    }
}
