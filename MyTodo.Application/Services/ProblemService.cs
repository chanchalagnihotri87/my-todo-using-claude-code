using Microsoft.Extensions.Logging;
using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class ProblemService : IProblemService
    {
        private readonly IProblemRepository _problemRepository;
        private readonly ILogger<ProblemService> _logger;

        public ProblemService(IProblemRepository problemRepository, ILogger<ProblemService> logger)
        {
            _problemRepository = problemRepository;
            _logger = logger;
        }

        public async Task<List<ProblemDto>> GetByLifeAreaIdAsync(int lifeAreaId)
        {
            var problems = await _problemRepository.GetByLifeAreaIdAsync(lifeAreaId);
            return problems.Select(MapToDto).ToList();
        }

        public async Task<ProblemDto?> GetByIdAsync(int id)
        {
            var problem = await _problemRepository.GetByIdAsync(id);
            return problem == null ? null : MapToDto(problem);
        }

        public async Task<ProblemDto> CreateAsync(CreateProblemDto createProblemDto)
        {
            var problem = new Problem
            {
                LifeAreaId = createProblemDto.LifeAreaId,
                Name = createProblemDto.Name,
                Description = createProblemDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _problemRepository.AddAsync(problem);

            _logger.LogInformation("Problem {ProblemId} created for life area {LifeAreaId}", problem.Id, problem.LifeAreaId);

            return MapToDto(problem);
        }

        public async Task<ProblemDto?> UpdateAsync(UpdateProblemDto updateProblemDto)
        {
            var problem = await _problemRepository.GetByIdAsync(updateProblemDto.Id);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found for update", updateProblemDto.Id);
                return null;
            }

            problem.Name = updateProblemDto.Name;
            problem.Description = updateProblemDto.Description;
            problem.Status = updateProblemDto.Status;
            problem.IsUrgent = updateProblemDto.IsUrgent;
            problem.IsImportant = updateProblemDto.IsImportant;
            problem.UpdatedAt = DateTime.UtcNow;

            await _problemRepository.UpdateAsync(problem);

            return MapToDto(problem);
        }

        public async Task<ProblemDto?> UpdateStatusAsync(int id, ProblemStatus status)
        {
            var problem = await _problemRepository.GetByIdAsync(id);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found for status update", id);
                return null;
            }

            _logger.LogInformation("Problem {ProblemId} status changed from {OldStatus} to {NewStatus}", problem.Id, problem.Status, status);

            problem.Status = status;
            problem.UpdatedAt = DateTime.UtcNow;

            await _problemRepository.UpdateAsync(problem);

            return MapToDto(problem);
        }

        public async Task<ProblemDto?> ToggleUrgentAsync(int id)
        {
            var problem = await _problemRepository.GetByIdAsync(id);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found for urgent toggle", id);
                return null;
            }

            problem.IsUrgent = !problem.IsUrgent;
            problem.UpdatedAt = DateTime.UtcNow;

            await _problemRepository.UpdateAsync(problem);

            return MapToDto(problem);
        }

        public async Task<ProblemDto?> ToggleImportantAsync(int id)
        {
            var problem = await _problemRepository.GetByIdAsync(id);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found for important toggle", id);
                return null;
            }

            problem.IsImportant = !problem.IsImportant;
            problem.UpdatedAt = DateTime.UtcNow;

            await _problemRepository.UpdateAsync(problem);

            return MapToDto(problem);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var problem = await _problemRepository.GetByIdAsync(id);
            if (problem == null)
            {
                _logger.LogWarning("Problem {ProblemId} not found for delete", id);
                return false;
            }

            await _problemRepository.DeleteAsync(problem);

            _logger.LogInformation("Problem {ProblemId} deleted", id);

            return true;
        }

        private static ProblemDto MapToDto(Problem problem)
        {
            return new ProblemDto
            {
                Id = problem.Id,
                Name = problem.Name,
                Description = problem.Description,
                Status = problem.Status,
                IsUrgent = problem.IsUrgent,
                IsImportant = problem.IsImportant,
                CreatedAt = problem.CreatedAt,
                UpdatedAt = problem.UpdatedAt,
                LifeAreaId = problem.LifeAreaId
            };
        }
    }
}
