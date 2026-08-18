using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repositories.Interfaces;
using MyTodo.Domain.Entities;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class TodoTaskRepository : BaseRepository<TodoTask>, ITodoTaskRepository
    {
        public TodoTaskRepository(MyTodoDbContext context) : base(context)
        {
        }

        public async Task<List<TodoTask>> GetByObjectiveIdAsync(int objectiveId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Sprint)
                .Where(x => x.ObjectiveId == objectiveId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TodoTask>> GetBySprintIdAsync(int sprintId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Objective)
                .Include(x => x.Todo)
                .Where(x => x.SprintId == sprintId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
