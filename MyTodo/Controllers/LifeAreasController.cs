using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.Services.Interfaces;

namespace MyTodo.Controllers
{
    public class LifeAreasController : Controller
    {
        private readonly ILifeAreaService _lifeAreaService;

        public LifeAreasController(ILifeAreaService lifeAreaService)
        {
            _lifeAreaService = lifeAreaService;
        }

        public async Task<IActionResult> Index()
        {
            var lifeAreas = await _lifeAreaService.GetAllAsync();
            return View(lifeAreas);
        }
    }
}
