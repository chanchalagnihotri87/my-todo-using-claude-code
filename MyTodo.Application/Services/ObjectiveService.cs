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

        public ObjectiveService(IObjectiveRepository objectiveRepository)
        {
            _objectiveRepository = objectiveRepository;
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
            var existing = await _objectiveRepository.GetBySolutionIdAsync(createObjectiveDto.SolutionId);

            var objective = new Objective
            {
                SolutionId = createObjectiveDto.SolutionId,
                Text = createObjectiveDto.Text,
                SortOrder = existing.Count,
                CreatedAt = DateTime.UtcNow
            };

            await _objectiveRepository.AddAsync(objective);

            return MapToDto(objective);
        }

        public async Task<ObjectiveDto?> UpdateAsync(UpdateObjectiveDto updateObjectiveDto)
        {
            var objective = await _objectiveRepository.GetByIdAsync(updateObjectiveDto.Id);
            if (objective == null)
            {
                return null;
            }

            objective.Text = updateObjectiveDto.Text;
            objective.Status = updateObjectiveDto.Status;
            objective.CompletedAt = updateObjectiveDto.Status == ObjectiveStatus.Completed ? DateTime.UtcNow : null;
            await _objectiveRepository.UpdateAsync(objective);

            return MapToDto(objective);
        }

        public async Task<bool> ReorderAsync(int id, ObjectiveStatus status, List<int> orderedIds)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null)
            {
                return false;
            }

            objective.Status = status;
            objective.CompletedAt = status == ObjectiveStatus.Completed ? DateTime.UtcNow : null;
            await _objectiveRepository.UpdateAsync(objective);

            await ReorderHelper.ReindexAsync(_objectiveRepository, objective, id, orderedIds, (entity, index) =>
            {
                entity.SortOrder = index;
            });

            return true;
        }

        public async Task<bool> ReorderFocusAsync(int id, bool isTwentyPercent, List<int> orderedIds)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null)
            {
                return false;
            }

            objective.IsTwentyPercent = isTwentyPercent;
            await _objectiveRepository.UpdateAsync(objective);

            await ReorderHelper.ReindexAsync(_objectiveRepository, objective, id, orderedIds, (entity, index) =>
            {
                entity.SortOrder = index;
            });

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null)
            {
                return false;
            }

            await _objectiveRepository.DeleteAsync(objective);

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
