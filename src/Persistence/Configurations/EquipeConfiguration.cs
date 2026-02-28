using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EquipeConfiguration : IEntityTypeConfiguration<Equipe>
{
    public void Configure(EntityTypeBuilder<Equipe> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nom)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(e => e.Membres)
            .WithMany()
            .UsingEntity(j => j.ToTable("EquipeMembres"));
    }
}
