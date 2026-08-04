using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class ProblemsController : Controller
    {
        private readonly IProblemService _problemService;
        private readonly ILifeAreaService _lifeAreaService;

        public ProblemsController(IProblemService problemService, ILifeAreaService lifeAreaService)
        {
            _problemService = problemService;
            _lifeAreaService = lifeAreaService;
        }

        public async Task<IActionResult> Index(int lifeAreaId)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(lifeAreaId);
            if (lifeArea == null)
            {
                return NotFound();
            }

            ViewBag.LifeArea = lifeArea;

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
    }
}
