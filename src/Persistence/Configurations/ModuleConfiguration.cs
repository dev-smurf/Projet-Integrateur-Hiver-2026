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

            builder.Property(m => m.Nom)
                .IsRequired();

            builder.Property(m => m.Contenu)
                .IsRequired();

            builder.Property(m => m.Sujet)
                .IsRequired();
        }
    }
}