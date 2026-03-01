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

            builder.Property(m => m.NameFr)
       .IsRequired()
       .HasMaxLength(100);

builder.Property(m => m.NameEn)
       .IsRequired()
       .HasMaxLength(100);

builder.Property(m => m.ContenueFr)
       .HasMaxLength(1000);

builder.Property(m => m.ContenueEn)
       .HasMaxLength(1000);

builder.Property(m => m.SujetFr)
       .HasMaxLength(200);

builder.Property(m => m.SujetEn)
       .HasMaxLength(200);
       builder.Property(m=>m.CardImageUrl).HasMaxLength(1000);

        }
    }
}
