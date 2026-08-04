using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;

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

        private static ProblemDto MapToDto(Problem problem)
        {
            return new ProblemDto
            {
                Id = problem.Id,
                Name = problem.Name,
                Description = problem.Description,
                Status = problem.Status,
                CreatedAt = problem.CreatedAt,
                UpdatedAt = problem.UpdatedAt,
                LifeAreaId = problem.LifeAreaId
            };
        }
    }
}
