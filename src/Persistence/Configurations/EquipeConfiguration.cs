using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EquipeConfiguration : IEntityTypeConfiguration<Equipe>
{
    public void Configure(EntityTypeBuilder<Equipe> builder)
    {
        builder.ToTable("Equipes"); // ✅ Nom explicite aligné avec le DbSet

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NameFr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(e => e.Membres)
            .WithMany()
            .UsingEntity(j => j.ToTable("EquipeMembres"));

        // ✅ Relation parent/sous-équipes
        builder.HasMany(e => e.SousEquipes)
            .WithOne(e => e.ParentEquipe)
            .HasForeignKey(e => e.ParentEquipeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
