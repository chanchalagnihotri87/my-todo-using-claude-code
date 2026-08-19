using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Common;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class ExperimentService : IExperimentService
    {
        private readonly IExperimentRepository _experimentRepository;

        public ExperimentService(IExperimentRepository experimentRepository)
        {
            _experimentRepository = experimentRepository;
        }

        public async Task<List<ExperimentDto>> GetBySolutionIdAsync(int solutionId)
        {
            var experiments = await _experimentRepository.GetBySolutionIdAsync(solutionId);
            return experiments.Select(MapToDto).ToList();
        }

        public async Task<ExperimentDto> CreateAsync(CreateExperimentDto createExperimentDto)
        {
            var count = await _experimentRepository.CountBySolutionIdAsync(createExperimentDto.SolutionId);

            var experiment = new Experiment
            {
                SolutionId = createExperimentDto.SolutionId,
                Name = createExperimentDto.Name,
                Description = createExperimentDto.Description,
                SortOrder = count,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            await _experimentRepository.AddAsync(experiment);

            return MapToDto(experiment);
        }

        public async Task<ExperimentDto?> UpdateAsync(UpdateExperimentDto updateExperimentDto)
        {
            var experiment = await _experimentRepository.GetByIdAsync(updateExperimentDto.Id);
            if (experiment == null)
            {
                return null;
            }

            experiment.Name = updateExperimentDto.Name;
            experiment.Description = updateExperimentDto.Description;
            experiment.Status = updateExperimentDto.Status;
            experiment.LastUpdatedAt = DateTime.UtcNow;
            await _experimentRepository.UpdateAsync(experiment);

            return MapToDto(experiment);
        }

        public async Task<bool> ReorderAsync(int id, ExperimentStatus status, List<int> orderedIds)
        {
            return await ReorderHelper.ReindexAsync(
                _experimentRepository,
                x => x.Id,
                orderedIds,
                (entity, index) => entity.SortOrder = index,
                anchorId: id,
                applyToAnchor: entity =>
                {
                    entity.Status = status;
                    entity.LastUpdatedAt = DateTime.UtcNow;
                });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var experiment = await _experimentRepository.GetByIdAsync(id);
            if (experiment == null)
            {
                return false;
            }

            await _experimentRepository.DeleteAsync(experiment);

            return true;
        }

        private static ExperimentDto MapToDto(Experiment experiment)
        {
            return new ExperimentDto
            {
                Id = experiment.Id,
                SolutionId = experiment.SolutionId,
                Name = experiment.Name,
                Description = experiment.Description,
                Status = experiment.Status,
                SortOrder = experiment.SortOrder,
                CreatedAt = experiment.CreatedAt,
                LastUpdatedAt = experiment.LastUpdatedAt
            };
        }
    }
}
