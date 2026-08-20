using Microsoft.AspNetCore.Mvc;
using MyTodo.Application.Services.Interfaces;

namespace MyTodo.Controllers
{
    public class ExploreController : Controller
    {
        private readonly ILifeAreaService _lifeAreaService;
        private readonly IProblemService _problemService;
        private readonly ISolutionService _solutionService;
        private readonly IObjectiveService _objectiveService;
        private readonly ITodoTaskService _todoTaskService;
        private readonly ILogger<ExploreController> _logger;

        public ExploreController(
            ILifeAreaService lifeAreaService,
            IProblemService problemService,
            ISolutionService solutionService,
            IObjectiveService objectiveService,
            ITodoTaskService todoTaskService,
            ILogger<ExploreController> logger)
        {
            _lifeAreaService = lifeAreaService;
            _problemService = problemService;
            _solutionService = solutionService;
            _objectiveService = objectiveService;
            _todoTaskService = todoTaskService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var lifeAreas = await _lifeAreaService.GetAllAsync();
            return View(lifeAreas);
        }

        [HttpGet]
        public async Task<IActionResult> Problems(int lifeAreaId)
        {
            var problems = await _problemService.GetByLifeAreaIdAsync(lifeAreaId);
            return PartialView("_ProblemNodes", problems);
        }

        [HttpGet]
        public async Task<IActionResult> Solutions(int problemId)
        {
            var solutions = await _solutionService.GetByProblemIdAsync(problemId);
            return PartialView("_SolutionNodes", solutions);
        }

        [HttpGet]
        public async Task<IActionResult> Objectives(int solutionId)
        {
            var objectives = await _objectiveService.GetBySolutionIdAsync(solutionId);
            return PartialView("_ObjectiveNodes", objectives);
        }

        [HttpGet]
        public async Task<IActionResult> Tasks(int objectiveId)
        {
            var tasks = await _todoTaskService.GetByObjectiveIdAsync(objectiveId);
            return PartialView("_TaskNodes", tasks);
        }
    }
}
