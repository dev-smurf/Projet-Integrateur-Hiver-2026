using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EquipeMessageConfiguration : IEntityTypeConfiguration<EquipeMessage>
{
    public void Configure(EntityTypeBuilder<EquipeMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Expediteur)
            .WithMany()
            .HasForeignKey(m => m.ExpediteurId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Reads)
            .WithOne(r => r.EquipeMessage)
            .HasForeignKey(r => r.EquipeMessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
