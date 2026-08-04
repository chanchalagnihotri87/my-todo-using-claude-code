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

        private static ProblemDto MapToDto(Problem problem)
        {
            return new ProblemDto
            {
                Id = problem.Id,
                Name = problem.Name,
                Description = problem.Description,
                CreatedAt = problem.CreatedAt,
                UpdatedAt = problem.UpdatedAt,
                LifeAreaId = problem.LifeAreaId
            };
        }
    }
}
