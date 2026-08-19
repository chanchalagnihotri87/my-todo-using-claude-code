using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;
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

        public async Task<int> CountBySolutionIdAsync(int solutionId)
        {
            return await _dbSet.CountAsync(x => x.SolutionId == solutionId);
        }

        public async Task<Dictionary<int, (int Total, int Completed)>> GetObjectiveCountsBySolutionIdsAsync(IEnumerable<int> solutionIds)
        {
            var idList = solutionIds as ICollection<int> ?? solutionIds.ToList();

            var counts = await _dbSet.AsNoTracking()
                .Where(x => idList.Contains(x.SolutionId))
                .GroupBy(x => x.SolutionId)
                .Select(g => new
                {
                    SolutionId = g.Key,
                    Total = g.Count(),
                    Completed = g.Count(o => o.Status == ObjectiveStatus.Completed)
                })
                .ToListAsync();

            return counts.ToDictionary(x => x.SolutionId, x => (x.Total, x.Completed));
        }
    }
}
