using Microsoft.Extensions.Logging;
using MyTodo.Application.DTOs;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;

namespace MyTodo.Application.Services
{
    public class LifeAreaService : ILifeAreaService
    {
        private readonly ILifeAreaRepository _lifeAreaRepository;
        private readonly ILogger<LifeAreaService> _logger;

        public LifeAreaService(ILifeAreaRepository lifeAreaRepository, ILogger<LifeAreaService> logger)
        {
            _lifeAreaRepository = lifeAreaRepository;
            _logger = logger;
        }

        public async Task<List<LifeAreaDto>> GetAllAsync()
        {
            var lifeAreas = await _lifeAreaRepository.GetAllAsync();
            return lifeAreas.Select(MapToDto).ToList();
        }

        public async Task<LifeAreaDto?> GetByIdAsync(int id)
        {
            var lifeArea = await _lifeAreaRepository.GetByIdAsync(id);
            return lifeArea == null ? null : MapToDto(lifeArea);
        }

        public async Task<LifeAreaDto> CreateAsync(CreateLifeAreaDto createLifeAreaDto)
        {
            var lifeArea = new LifeArea
            {
                Name = createLifeAreaDto.Name,
                Description = createLifeAreaDto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _lifeAreaRepository.AddAsync(lifeArea);

            _logger.LogInformation("LifeArea {LifeAreaId} created", lifeArea.Id);

            return MapToDto(lifeArea);
        }

        public async Task<LifeAreaDto?> UpdateAsync(UpdateLifeAreaDto updateLifeAreaDto)
        {
            var lifeArea = await _lifeAreaRepository.GetByIdAsync(updateLifeAreaDto.Id);
            if (lifeArea == null)
            {
                _logger.LogWarning("LifeArea {LifeAreaId} not found for update", updateLifeAreaDto.Id);
                return null;
            }

            lifeArea.Name = updateLifeAreaDto.Name;
            lifeArea.Description = updateLifeAreaDto.Description;
            lifeArea.UpdatedAt = DateTime.UtcNow;

            await _lifeAreaRepository.UpdateAsync(lifeArea);

            return MapToDto(lifeArea);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lifeArea = await _lifeAreaRepository.GetByIdAsync(id);
            if (lifeArea == null)
            {
                _logger.LogWarning("LifeArea {LifeAreaId} not found for delete", id);
                return false;
            }

            await _lifeAreaRepository.DeleteAsync(lifeArea);

            _logger.LogInformation("LifeArea {LifeAreaId} deleted", id);

            return true;
        }

        private static LifeAreaDto MapToDto(LifeArea lifeArea)
        {
            return new LifeAreaDto
            {
                Id = lifeArea.Id,
                Name = lifeArea.Name,
                Description = lifeArea.Description,
                CreatedAt = lifeArea.CreatedAt,
                UpdatedAt = lifeArea.UpdatedAt
            };
        }
    }
}
