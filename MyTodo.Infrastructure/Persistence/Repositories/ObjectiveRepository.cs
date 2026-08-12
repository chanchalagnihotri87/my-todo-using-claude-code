using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repository.Interface;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class ObjectiveRepository : BaseRepository<Objective>, IObjectiveRepository
    {
        public ObjectiveRepository(MyTodoDbContext context) : base(context)
        {
        }

        public async Task<List<Objective>> GetBySolutionIdAsync(int solutionId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.SolutionId == solutionId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
