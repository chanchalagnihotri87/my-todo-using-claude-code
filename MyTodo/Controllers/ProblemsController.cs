using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Enums;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ProblemsController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ILifeAreaService _lifeAreaService;
        private readonly IProblemStatusOrderService _problemStatusOrderService;

        public ProblemsController(IProblemService problemService, ILifeAreaService lifeAreaService, IProblemStatusOrderService problemStatusOrderService)
        {
            _problemService = problemService;
            _lifeAreaService = lifeAreaService;
            _problemStatusOrderService = problemStatusOrderService;
        }

        public async Task<IActionResult> Index(int lifeAreaId)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(lifeAreaId);
            if (lifeArea == null)
            {
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

            return RedirectToAction(nameof(Index), new { lifeAreaId = model.LifeAreaId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UpdateProblemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 200)
            {
                return BadRequest();
            }

            if (!Enum.TryParse<ProblemStatus>(request.Status, out var status))
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
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteProblemRequest request)
        {
            var deleted = await _problemService.DeleteAsync(request.Id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateProblemStatusRequest request)
        {
            if (!Enum.TryParse<ProblemStatus>(request.Status, out var status))
            {
                return BadRequest();
            }

            var updated = await _problemService.UpdateStatusAsync(request.Id, status);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderLists([FromBody] ReorderListsRequest request)
        {
            var statuses = new List<ProblemStatus>();
            foreach (var value in request.OrderedStatuses)
            {
                if (!Enum.TryParse<ProblemStatus>(value, out var status))
                {
                    return BadRequest();
                }
                statuses.Add(status);
            }

            await _problemStatusOrderService.ReorderAsync(statuses);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUrgent([FromBody] ToggleProblemTagRequest request)
        {
            var updated = await _problemService.ToggleUrgentAsync(request.Id);
            if (updated == null)
            {
                return NotFound();
            }

            return Json(new { isUrgent = updated.IsUrgent });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleImportant([FromBody] ToggleProblemTagRequest request)
        {
            var updated = await _problemService.ToggleImportantAsync(request.Id);
            if (updated == null)
            {
                return NotFound();
            }

            return Json(new { isImportant = updated.IsImportant });
        }
    }
}
