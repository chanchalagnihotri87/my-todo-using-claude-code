using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class LifeAreaRepository : BaseRepository<LifeArea>, ILifeAreaRepository
    {
        public LifeAreaRepository(MyTodoDbContext context) : base(context)
        {
        }

        public override async Task<List<LifeArea>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }
    }
}
