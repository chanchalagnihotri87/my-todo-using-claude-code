using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTodo.Domain.Entities;

namespace MyTodo.Infrastructure.Persistence.Configurations
{
    public class ProblemStatusOrderConfiguration : IEntityTypeConfiguration<ProblemStatusOrder>
    {
        public void Configure(EntityTypeBuilder<ProblemStatusOrder> builder)
        {
            builder.ToTable("ProblemStatusOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            builder.HasIndex(x => x.Status)
                .IsUnique();
        }
    }
}
