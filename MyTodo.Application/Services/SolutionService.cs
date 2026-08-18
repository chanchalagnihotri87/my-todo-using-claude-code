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

        public SolutionService(ISolutionRepository solutionRepository)
        {
            _solutionRepository = solutionRepository;
        }

        public async Task<List<SolutionDto>> GetByProblemIdAsync(int problemId)
        {
            var solutions = await _solutionRepository.GetByProblemIdAsync(problemId);
            return solutions.Select(MapToDto).ToList();
        }

        public async Task<SolutionDto?> GetByIdAsync(int id)
        {
            var solution = await _solutionRepository.GetByIdAsync(id);
            return solution == null ? null : MapToDto(solution);
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
            var solution = await _solutionRepository.GetByIdAsync(id);
            if (solution == null)
            {
                return false;
            }

            solution.Status = status;
            solution.UpdatedAt = DateTime.UtcNow;
            await _solutionRepository.UpdateAsync(solution);

            await ReorderHelper.ReindexAsync(_solutionRepository, solution, id, orderedIds, (entity, index) =>
            {
                entity.SortOrder = index;
                entity.UpdatedAt = DateTime.UtcNow;
            });

            return true;
        }

        public async Task<bool> ReorderTwentyPercentAsync(int id, bool isTwentyPercent, List<int> orderedIds)
        {
            var solution = await _solutionRepository.GetByIdAsync(id);
            if (solution == null)
            {
                return false;
            }

            solution.IsTwentyPercent = isTwentyPercent;
            solution.UpdatedAt = DateTime.UtcNow;
            await _solutionRepository.UpdateAsync(solution);

            await ReorderHelper.ReindexAsync(_solutionRepository, solution, id, orderedIds, (entity, index) =>
            {
                entity.SortOrder = index;
                entity.UpdatedAt = DateTime.UtcNow;
            });

            return true;
        }

        private static SolutionDto MapToDto(Solution solution)
        {
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
                TotalObjectivesCount = solution.Objectives.Count,
                CompletedObjectivesCount = solution.Objectives.Count(o => o.Status == ObjectiveStatus.Completed)
            };
        }
    }
}
