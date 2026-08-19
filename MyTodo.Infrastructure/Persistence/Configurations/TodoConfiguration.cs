using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTodo.Domain.Entities;

namespace MyTodo.Infrastructure.Persistence.Configurations
{
    public class TodoConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.ToTable("Todos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TodoDate)
                .IsRequired();

            builder.Property(x => x.IsUrgent)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsImportant)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.IsFrog)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.TodoDate);

            builder.HasOne(x => x.TodoTask)
                .WithOne(x => x.Todo)
                .HasForeignKey<Todo>(x => x.TodoTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
