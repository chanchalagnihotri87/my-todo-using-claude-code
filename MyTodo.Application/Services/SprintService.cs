using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;

namespace MyTodo.Application.Services
{
    public class SprintService : ISprintService
    {
        private readonly ISprintRepository _sprintRepository;

        public SprintService(ISprintRepository sprintRepository)
        {
            _sprintRepository = sprintRepository;
        }

        public async Task<List<SprintDto>> GetAllAsync()
        {
            var sprints = await _sprintRepository.GetAllAsync();
            return sprints.Select(MapToDto).ToList();
        }

        public async Task<SprintDto> CreateAsync(CreateSprintDto createSprintDto)
        {
            var sprint = new Sprint
            {
                Name = createSprintDto.Name,
                Description = createSprintDto.Description,
                StartDate = createSprintDto.StartDate,
                EndDate = createSprintDto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            await _sprintRepository.AddAsync(sprint);

            return MapToDto(sprint);
        }

        public async Task<SprintDto?> GetByIdAsync(int id)
        {
            var sprint = await _sprintRepository.GetByIdAsync(id);
            return sprint == null ? null : MapToDto(sprint);
        }

        public async Task<SprintDto?> GetCurrentAsync()
        {
            var sprint = await _sprintRepository.GetCurrentAsync(DateTime.UtcNow.Date);
            return sprint == null ? null : MapToDto(sprint);
        }

        public async Task<SprintDto?> UpdateAsync(UpdateSprintDto updateSprintDto)
        {
            var sprint = await _sprintRepository.GetByIdAsync(updateSprintDto.Id);
            if (sprint == null)
            {
                return null;
            }

            sprint.Name = updateSprintDto.Name;
            sprint.Description = updateSprintDto.Description;
            sprint.StartDate = updateSprintDto.StartDate;
            sprint.EndDate = updateSprintDto.EndDate;
            sprint.UpdatedAt = DateTime.UtcNow;
            await _sprintRepository.UpdateAsync(sprint);

            return MapToDto(sprint);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sprint = await _sprintRepository.GetByIdAsync(id);
            if (sprint == null)
            {
                return false;
            }

            await _sprintRepository.DeleteAsync(sprint);

            return true;
        }

        private static SprintDto MapToDto(Sprint sprint)
        {
            return new SprintDto
            {
                Id = sprint.Id,
                Name = sprint.Name,
                Description = sprint.Description,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate,
                CreatedAt = sprint.CreatedAt,
                UpdatedAt = sprint.UpdatedAt
            };
        }
    }
}
