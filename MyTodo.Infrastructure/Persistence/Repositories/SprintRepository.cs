using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class SprintRepository : BaseRepository<Sprint>, ISprintRepository
    {
        public SprintRepository(MyTodoDbContext context) : base(context)
        {
        }

        public override async Task<List<Sprint>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().OrderByDescending(x => x.StartDate).ToListAsync();
        }

        public async Task<Sprint?> GetCurrentAsync(DateTime today)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.StartDate <= today && x.EndDate >= today)
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefaultAsync();
        }
    }
}
