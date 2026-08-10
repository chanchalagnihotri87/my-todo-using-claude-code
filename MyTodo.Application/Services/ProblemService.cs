using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Application.Services
{
    public class ProblemService : IProblemService
    {
        private readonly IProblemRepository _problemRepository;

        public ProblemService(IProblemRepository problemRepository)
        {
            _problemRepository = problemRepository;
        }

        public async Task<List<ProblemDto>> GetByLifeAreaIdAsync(int lifeAreaId)
        {
            var problems = await _problemRepository.GetByLifeAreaIdAsync(lifeAreaId);
            return problems.Select(MapToDto).ToList();
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

            return MapToDto(problem);
        }

        public async Task<ProblemDto?> UpdateAsync(UpdateProblemDto updateProblemDto)
        {
            var problem = await _problemRepository.GetByIdAsync(updateProblemDto.Id);
            if (problem == null)
            {
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
                return null;
            }

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
                return false;
            }

            await _problemRepository.DeleteAsync(problem);

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
