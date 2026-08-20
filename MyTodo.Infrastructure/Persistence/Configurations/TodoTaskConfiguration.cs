using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Shared.Enums;

namespace MyTodo.Infrastructure.Persistence.Configurations
{
    public class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
    {
        public void Configure(EntityTypeBuilder<TodoTask> builder)
        {
            builder.ToTable("TodoTasks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(TodoStatus.Pending)
                .HasSentinel(TodoStatus.Pending);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.HasOne(x => x.Objective)
                .WithMany()
                .HasForeignKey(x => x.ObjectiveId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Sprint)
                .WithMany(x => x.TodoTasks)
                .HasForeignKey(x => x.SprintId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
