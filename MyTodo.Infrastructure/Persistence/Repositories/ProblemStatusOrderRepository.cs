using Microsoft.EntityFrameworkCore;
using MyTodo.Application.Repository.Interface;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;
using MyTodo.Infrastructure.Persistence.DbContext;

namespace MyTodo.Infrastructure.Persistence.Repositories
{
    public class ProblemStatusOrderRepository : BaseRepository<ProblemStatusOrder>, IProblemStatusOrderRepository
    {
        private readonly MyTodoDbContext _context;

        public ProblemStatusOrderRepository(MyTodoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ProblemStatusOrder>> GetAllOrderedAsync()
        {
            return await _dbSet.AsNoTracking().OrderBy(x => x.SortOrder).ToListAsync();
        }

        public async Task ReorderAsync(List<ProblemStatus> orderedStatuses)
        {
            var existing = await _dbSet.ToListAsync();

            for (int i = 0; i < orderedStatuses.Count; i++)
            {
                var row = existing.FirstOrDefault(x => x.Status == orderedStatuses[i]);
                if (row == null)
                {
                    _dbSet.Add(new ProblemStatusOrder { Status = orderedStatuses[i], SortOrder = i });
                }
                else
                {
                    row.SortOrder = i;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
