using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
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

        public async Task<bool> UpdateStatusAsync(int id, ObjectiveStatus status)
        {
            var objective = await _objectiveRepository.GetByIdAsync(id);
            if (objective == null)
            {
                return false;
            }

            objective.Status = status;
            objective.CompletedAt = status == ObjectiveStatus.Completed ? DateTime.UtcNow : null;
            await _objectiveRepository.UpdateAsync(objective);

            return true;
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

            for (var index = 0; index < orderedIds.Count; index++)
            {
                var current = orderedIds[index] == id ? objective : await _objectiveRepository.GetByIdAsync(orderedIds[index]);
                if (current == null)
                {
                    continue;
                }

                current.SortOrder = index;
                await _objectiveRepository.UpdateAsync(current);
            }

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
                CompletedAt = objective.CompletedAt,
                SortOrder = objective.SortOrder,
                CreatedAt = objective.CreatedAt
            };
        }
    }
}
