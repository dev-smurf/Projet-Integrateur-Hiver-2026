using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ArchiveConfiguration : IEntityTypeConfiguration<Archive>
{
    public void Configure(EntityTypeBuilder<Archive> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Progression)
            .WithMany()
            .HasForeignKey(a => a.ProgressionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Rdv)
            .WithMany()
            .HasForeignKey(a => a.RdvId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Equipe)
            .WithMany()
            .HasForeignKey(a => a.EquipeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
