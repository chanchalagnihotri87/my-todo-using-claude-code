using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repository.Interface;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class ProblemRepository : BaseRepository<Problem>, IProblemRepository
    {
        public ProblemRepository(MyTodoDbContext context) : base(context)
        {
        }

        public async Task<List<Problem>> GetByLifeAreaIdAsync(int lifeAreaId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.LifeAreaId == lifeAreaId)
                .OrderBy(x => x.IsUrgent && x.IsImportant ? 0 : x.IsImportant ? 1 : x.IsUrgent ? 2 : 3)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
    }
}
