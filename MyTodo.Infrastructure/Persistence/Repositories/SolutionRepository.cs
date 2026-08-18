using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class SolutionRepository : BaseRepository<Solution>, ISolutionRepository
    {
        public SolutionRepository(MyTodoDbContext context) : base(context)
        {
        }

        public async Task<List<Solution>> GetByProblemIdAsync(int problemId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Objectives)
                .Where(x => x.ProblemId == problemId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
    }
}
