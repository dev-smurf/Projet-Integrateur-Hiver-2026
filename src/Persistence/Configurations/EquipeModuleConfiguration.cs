using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Configurations
{
    public class EquipeModuleConfiguration : IEntityTypeConfiguration<EquipeModule>
    {
        public void Configure(EntityTypeBuilder<EquipeModule> builder)
        {
            builder.HasKey(x => new
            {
                x.EquipeId,
                x.ModuleId
            });

            builder
                .HasOne(x => x.Equipe)
                .WithMany(x => x.EquipeModules)
                .HasForeignKey(x => x.EquipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Module)
                .WithMany(x => x.EquipeModules)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                 .Property(x => x.AssignedAt)
            .IsRequired();
        }
    }
}
