using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.DTOs;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Models;

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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var lifeArea = await _lifeAreaService.GetByIdAsync(id);
            if (lifeArea == null)
            {
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
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _lifeAreaService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
