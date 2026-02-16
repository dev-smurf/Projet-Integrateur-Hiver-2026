using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class RdvConfiguration : IEntityTypeConfiguration<Rdv>
{
    public void Configure(EntityTypeBuilder<Rdv> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Titre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Note)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
