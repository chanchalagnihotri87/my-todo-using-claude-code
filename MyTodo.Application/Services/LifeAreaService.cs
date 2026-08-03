using MyTodo.Application.DTOs;
using MyTodo.Application.Repository.Interface;
using MyTodo.Application.Services.Interfaces;
using MyTodo.Domain.Entities;

namespace MyTodo.Application.Services
{
    public class LifeAreaService : ILifeAreaService
    {
        private readonly ILifeAreaRepository _lifeAreaRepository;

        public LifeAreaService(ILifeAreaRepository lifeAreaRepository)
        {
            _lifeAreaRepository = lifeAreaRepository;
        }

        public async Task<List<LifeAreaDto>> GetAllAsync()
        {
            var lifeAreas = await _lifeAreaRepository.GetAllAsync();
            return lifeAreas.Select(MapToDto).ToList();
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
