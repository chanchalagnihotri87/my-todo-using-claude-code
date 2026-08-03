using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTodo.Domain.Entities;

namespace MyTodo.Infrastructure.Persistence.Configurations
{
    public class LifeAreaConfiguration : IEntityTypeConfiguration<LifeArea>
    {
        public void Configure(EntityTypeBuilder<LifeArea> builder)
        {
            builder.ToTable("LifeAreas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                .IsRequired();
        }
    }
}
