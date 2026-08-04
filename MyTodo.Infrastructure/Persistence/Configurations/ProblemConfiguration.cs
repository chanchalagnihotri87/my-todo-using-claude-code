using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTodo.Domain.Entities;
using MyTodo.Domain.Enums;

namespace MyTodo.Infrastructure.Persistence.Configurations
{
    public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
    {
        public void Configure(EntityTypeBuilder<Problem> builder)
        {
            builder.ToTable("Problems");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ProblemStatus.Pending);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.LifeArea)
                .WithMany(x => x.Problems)
                .HasForeignKey(x => x.LifeAreaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
