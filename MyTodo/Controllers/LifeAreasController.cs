using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class LifeAreasController : Controller
    {
        private readonly ILifeAreaService _lifeAreaService;
        private readonly ILogger<LifeAreasController> _logger;

        public LifeAreasController(ILifeAreaService lifeAreaService, ILogger<LifeAreasController> logger)
        {
            _lifeAreaService = lifeAreaService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var lifeAreas = await _lifeAreaService.GetAllAsync();
            return View(lifeAreas);
        }

        public IActionResult Create()
        {
            return View(new CreateLifeAreaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLifeAreaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var createLifeAreaDto = new CreateLifeAreaDto
            {
                Name = model.Name,
                Description = model.Description
            };

            await _lifeAreaService.CreateAsync(createLifeAreaDto);
            _logger.LogInformation("Created life area {LifeAreaName}", model.Name);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(id);
            if (lifeArea == null)
            {
                _logger.LogWarning("Life area {LifeAreaId} not found when loading edit page", id);
                return NotFound();
            }

            var model = new EditLifeAreaViewModel
            {
                Id = lifeArea.Id,
                Name = lifeArea.Name,
                Description = lifeArea.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditLifeAreaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updateLifeAreaDto = new UpdateLifeAreaDto
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            };

            var updated = await _lifeAreaService.UpdateAsync(updateLifeAreaDto);
            if (updated == null)
            {
                _logger.LogWarning("Life area {LifeAreaId} not found when editing", model.Id);
                return NotFound();
            }

            _logger.LogInformation("Updated life area {LifeAreaId}", model.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _lifeAreaService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Life area {LifeAreaId} not found when deleting", id);
                return NotFound();
            }

            _logger.LogInformation("Deleted life area {LifeAreaId}", id);
            return RedirectToAction(nameof(Index));
        }
    }
}
