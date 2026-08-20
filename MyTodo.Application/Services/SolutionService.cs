using Microsoft.Extensions.Logging;
using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Common;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Application.Services
{
    public class SolutionService : ISolutionService
    {
        private readonly ISolutionRepository _solutionRepository;
        private readonly IObjectiveRepository _objectiveRepository;
        private readonly ILogger<SolutionService> _logger;

        public SolutionService(ISolutionRepository solutionRepository, IObjectiveRepository objectiveRepository, ILogger<SolutionService> logger)
        {
            _solutionRepository = solutionRepository;
            _objectiveRepository = objectiveRepository;
            _logger = logger;
        }

        public async Task<List<SolutionDto>> GetByProblemIdAsync(int problemId)
        {
            var solutions = await _solutionRepository.GetByProblemIdAsync(problemId);
            var counts = await _objectiveRepository.GetObjectiveCountsBySolutionIdsAsync(solutions.Select(x => x.Id));
            return solutions.Select(s => MapToDto(s, counts)).ToList();
        }

        public async Task<SolutionDto?> GetByIdAsync(int id)
        {
            var solution = await _solutionRepository.GetByIdAsync(id);
            if (solution == null)
            {
                return null;
            }

            var counts = await _objectiveRepository.GetObjectiveCountsBySolutionIdsAsync(new[] { id });
            return MapToDto(solution, counts);
        }

        public async Task<SolutionDto> CreateAsync(CreateSolutionDto createSolutionDto)
        {
            var solution = new Solution
            {
                ProblemId = createSolutionDto.ProblemId,
                Name = createSolutionDto.Name,
                Description = createSolutionDto.Description,
                IsTwentyPercent = createSolutionDto.IsTwentyPercent,
                CreatedAt = DateTime.UtcNow
            };

            await _solutionRepository.AddAsync(solution);

            _logger.LogInformation("Solution {SolutionId} created for problem {ProblemId}", solution.Id, solution.ProblemId);

            return MapToDto(solution);
        }

        public async Task<bool> ReorderAsync(int id, SolutionStatus status, List<int> orderedIds)
        {
            var anchorFound = await ReorderHelper.ReindexAsync(
                _solutionRepository,
                x => x.Id,
                orderedIds,
                (entity, index) =>
                {
                    entity.SortOrder = index;
                    entity.UpdatedAt = DateTime.UtcNow;
                },
                anchorId: id,
                applyToAnchor: entity => entity.Status = status);

            if (!anchorFound)
            {
                _logger.LogWarning("Solution {SolutionId} not found as reorder anchor", id);
            }

            return anchorFound;
        }

        public async Task<bool> ReorderTwentyPercentAsync(int id, bool isTwentyPercent, List<int> orderedIds)
        {
            var anchorFound = await ReorderHelper.ReindexAsync(
                _solutionRepository,
                x => x.Id,
                orderedIds,
                (entity, index) =>
                {
                    entity.SortOrder = index;
                    entity.UpdatedAt = DateTime.UtcNow;
                },
                anchorId: id,
                applyToAnchor: entity => entity.IsTwentyPercent = isTwentyPercent);

            if (!anchorFound)
            {
                _logger.LogWarning("Solution {SolutionId} not found as reorder-twenty-percent anchor", id);
            }

            return anchorFound;
        }

        private static SolutionDto MapToDto(Solution solution, Dictionary<int, (int Total, int Completed)>? counts = null)
        {
            var count = (Total: 0, Completed: 0);
            counts?.TryGetValue(solution.Id, out count);

            return new SolutionDto
            {
                Id = solution.Id,
                Name = solution.Name,
                Description = solution.Description,
                ProblemId = solution.ProblemId,
                IsTwentyPercent = solution.IsTwentyPercent,
                SortOrder = solution.SortOrder,
                Status = solution.Status,
                CreatedAt = solution.CreatedAt,
                UpdatedAt = solution.UpdatedAt,
                TotalObjectivesCount = count.Total,
                CompletedObjectivesCount = count.Completed
            };
        }
    }
}
