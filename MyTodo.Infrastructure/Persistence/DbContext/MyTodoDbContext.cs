using Microsoft.EntityFrameworkCore;
using MyTodo.Domain.Entities;

namespace MyTodo.Infrastructure.Persistence.DbContext
{
    public class MyTodoDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public MyTodoDbContext(DbContextOptions<MyTodoDbContext> options) : base(options)
        {
        }

        public DbSet<LifeArea> LifeAreas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyTodoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
