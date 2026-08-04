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
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
