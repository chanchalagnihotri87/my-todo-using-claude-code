using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repository.Interface;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class TodoRepository : BaseRepository<Todo>, ITodoRepository
    {
        public TodoRepository(MyTodoDbContext context) : base(context)
        {
        }

        public async Task<Todo?> GetByTodoTaskIdAsync(int todoTaskId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.TodoTaskId == todoTaskId);
        }

        public async Task<List<Todo>> GetByDateAsync(DateOnly date)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.TodoTask).ThenInclude(t => t.Objective)
                .Include(x => x.TodoTask).ThenInclude(t => t.Sprint)
                .Where(x => x.TodoDate == date)
                .OrderByDescending(x => x.IsFrog)
                .ThenByDescending(x => x.IsUrgent && x.IsImportant)
                .ThenByDescending(x => x.IsImportant)
                .ThenByDescending(x => x.IsUrgent)
                .ThenBy(x => x.TodoTask.Name)
                .ToListAsync();
        }

        public async Task<Todo?> GetFrogByDateAsync(DateOnly date)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TodoDate == date && x.IsFrog);
        }

        public async Task<List<Todo>> GetHistoryAsync(int? objectiveId, DateOnly? fromDate, DateOnly? toDate)
        {
            var query = _dbSet.AsNoTracking()
                .Include(x => x.TodoTask).ThenInclude(t => t.Objective)
                .Include(x => x.TodoTask).ThenInclude(t => t.Sprint)
                .AsQueryable();

            if (objectiveId.HasValue)
            {
                query = query.Where(x => x.TodoTask.ObjectiveId == objectiveId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.TodoDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.TodoDate <= toDate.Value);
            }

            return await query.OrderByDescending(x => x.TodoDate).ToListAsync();
        }
    }
}
