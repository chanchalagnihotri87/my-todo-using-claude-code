using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyTodo.Models;
using System.Diagnostics;

namespace MyTodo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>()?.Error;

            if (exception != null)
            {
                _logger.LogError(exception, "Unhandled exception for request {RequestId}", requestId);
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
