using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.Services.Interfaces;

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
    }
}
