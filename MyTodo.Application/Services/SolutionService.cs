using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Common;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class SolutionService : ISolutionService
    {
        private readonly ISolutionRepository _solutionRepository;
        private readonly IObjectiveRepository _objectiveRepository;

        public SolutionService(ISolutionRepository solutionRepository, IObjectiveRepository objectiveRepository)
        {
            _solutionRepository = solutionRepository;
            _objectiveRepository = objectiveRepository;
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

            return MapToDto(solution);
        }

        public async Task<bool> ReorderAsync(int id, SolutionStatus status, List<int> orderedIds)
        {
            return await ReorderHelper.ReindexAsync(
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
        }

        public async Task<bool> ReorderTwentyPercentAsync(int id, bool isTwentyPercent, List<int> orderedIds)
        {
            return await ReorderHelper.ReindexAsync(
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
