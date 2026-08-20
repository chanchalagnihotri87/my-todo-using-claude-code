using Microsoft.Extensions.Logging;
using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Common;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class ObjectiveService : IObjectiveService
    {
        private readonly IObjectiveRepository _objectiveRepository;
        private readonly ILogger<ObjectiveService> _logger;

        public ObjectiveService(IObjectiveRepository objectiveRepository, ILogger<ObjectiveService> logger)
        {
            _objectiveRepository = objectiveRepository;
            _logger = logger;
        }

        public async Task<List<ObjectiveDto>> GetBySolutionIdAsync(int solutionId)
        {
            var objectives = await _objectiveRepository.GetBySolutionIdAsync(solutionId);
            return objectives.Select(MapToDto).ToList();
        }

        public async Task<List<ObjectiveDto>> GetAllAsync()
        {
            var objectives = await _objectiveRepository.GetAllAsync();
            return objectives.Select(MapToDto).OrderBy(x => x.Text).ToList();
        }

        public async Task<ObjectiveDto?> GetByIdAsync(int id)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            return objective == null ? null : MapToDto(objective);
        }

        public async Task<ObjectiveDto> CreateAsync(CreateObjectiveDto createObjectiveDto)
        {
            var count = await _objectiveRepository.CountBySolutionIdAsync(createObjectiveDto.SolutionId);

            var objective = new Objective
            {
                SolutionId = createObjectiveDto.SolutionId,
                Text = createObjectiveDto.Text,
                SortOrder = count,
                CreatedAt = DateTime.UtcNow
            };

            await _objectiveRepository.AddAsync(objective);

            _logger.LogInformation("Objective {ObjectiveId} created for solution {SolutionId}", objective.Id, objective.SolutionId);

            return MapToDto(objective);
        }

        public async Task<ObjectiveDto?> UpdateAsync(UpdateObjectiveDto updateObjectiveDto)
        {
            var objective = await _objectiveRepository.GetByIdAsync(updateObjectiveDto.Id);
            if (objective == null)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found for update", updateObjectiveDto.Id);
                return null;
            }

            if (objective.Status != updateObjectiveDto.Status)
            {
                _logger.LogInformation("Objective {ObjectiveId} status changed from {OldStatus} to {NewStatus}", objective.Id, objective.Status, updateObjectiveDto.Status);
            }

            objective.Text = updateObjectiveDto.Text;
            objective.Status = updateObjectiveDto.Status;
            objective.CompletedAt = updateObjectiveDto.Status == ObjectiveStatus.Completed ? DateTime.UtcNow : null;
            await _objectiveRepository.UpdateAsync(objective);

            return MapToDto(objective);
        }

        public async Task<bool> ReorderAsync(int id, ObjectiveStatus status, List<int> orderedIds)
        {
            var anchorFound = await ReorderHelper.ReindexAsync(
                _objectiveRepository,
                x => x.Id,
                orderedIds,
                (entity, index) => entity.SortOrder = index,
                anchorId: id,
                applyToAnchor: entity =>
                {
                    entity.Status = status;
                    entity.CompletedAt = status == ObjectiveStatus.Completed ? DateTime.UtcNow : null;
                });

            if (!anchorFound)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found as reorder anchor", id);
            }

            return anchorFound;
        }

        public async Task<bool> ReorderFocusAsync(int id, bool isTwentyPercent, List<int> orderedIds)
        {
            var anchorFound = await ReorderHelper.ReindexAsync(
                _objectiveRepository,
                x => x.Id,
                orderedIds,
                (entity, index) => entity.SortOrder = index,
                anchorId: id,
                applyToAnchor: entity => entity.IsTwentyPercent = isTwentyPercent);

            if (!anchorFound)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found as reorder-focus anchor", id);
            }

            return anchorFound;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null)
            {
                _logger.LogWarning("Objective {ObjectiveId} not found for delete", id);
                return false;
            }

            await _objectiveRepository.DeleteAsync(objective);

            _logger.LogInformation("Objective {ObjectiveId} deleted", id);

            return true;
        }

        private static ObjectiveDto MapToDto(Objective objective)
        {
            return new ObjectiveDto
            {
                Id = objective.Id,
                SolutionId = objective.SolutionId,
                Text = objective.Text,
                Status = objective.Status,
                IsTwentyPercent = objective.IsTwentyPercent,
                CompletedAt = objective.CompletedAt,
                SortOrder = objective.SortOrder,
                CreatedAt = objective.CreatedAt
            };
        }
    }
}
