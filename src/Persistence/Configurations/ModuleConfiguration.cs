using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Content)
                .HasMaxLength(1000);

            builder.Property(m => m.Subject)
                .HasMaxLength(200);

            builder.Property(m => m.CardImageUrl)
                .HasMaxLength(1000);
        }
    }
}
