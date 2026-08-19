using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class ExperimentRepository : BaseRepository<Experiment>, IExperimentRepository
    {
        public ExperimentRepository(MyTodoDbContext context) : base(context)
        {
        }

        public async Task<List<Experiment>> GetBySolutionIdAsync(int solutionId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.SolutionId == solutionId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountBySolutionIdAsync(int solutionId)
        {
            return await _dbSet.CountAsync(x => x.SolutionId == solutionId);
        }
    }
}
