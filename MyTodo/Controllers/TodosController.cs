using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Models;

namespace MyTodo.Controllers
{
    public class TodosController : Controller
    {
        private readonly ITodoService _todoService;
        private readonly IObjectiveService _objectiveService;

        public TodosController(ITodoService todoService, IObjectiveService objectiveService)
        {
            _todoService = todoService;
            _objectiveService = objectiveService;
        }

        public async Task<IActionResult> Index()
        {
            var todos = await _todoService.GetTodayAsync();
            return View(todos);
        }

        public async Task<IActionResult> History(int? objectiveId, DateOnly? date, string? week)
        {
            DateOnly? fromDate = date;
            DateOnly? toDate = date;

            if (!string.IsNullOrWhiteSpace(week) && TryParseWeek(week, out var weekStart, out var weekEnd))
            {
                fromDate = weekStart;
                toDate = weekEnd;
            }

            var todos = await _todoService.GetHistoryAsync(objectiveId, fromDate, toDate);

            ViewBag.Objectives = await _objectiveService.GetAllAsync();
            ViewBag.SelectedObjectiveId = objectiveId;
            ViewBag.SelectedDate = date;
            ViewBag.SelectedWeek = week;

            return View(todos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUrgent([FromBody] ToggleTodoTagRequest request)
        {
            var updated = await _todoService.ToggleUrgentAsync(request.Id);
            if (updated == null)
            {
                return NotFound();
            }

            return Json(new { isUrgent = updated.IsUrgent });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleImportant([FromBody] ToggleTodoTagRequest request)
        {
            var updated = await _todoService.ToggleImportantAsync(request.Id);
            if (updated == null)
            {
                return NotFound();
            }

            return Json(new { isImportant = updated.IsImportant });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFrog([FromBody] ToggleTodoTagRequest request)
        {
            var updated = await _todoService.ToggleFrogAsync(request.Id);
            if (updated == null)
            {
                return NotFound();
            }

            return Json(new { isFrog = updated.IsFrog });
        }

        private static bool TryParseWeek(string week, out DateOnly weekStart, out DateOnly weekEnd)
        {
            weekStart = default;
            weekEnd = default;

            var parts = week.Split('-', 'W');
            if (parts.Length != 3 || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[2], out var weekNumber))
            {
                return false;
            }

            var monday = ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Monday);
            weekStart = DateOnly.FromDateTime(monday);
            weekEnd = weekStart.AddDays(6);

            return true;
        }
    }
}
