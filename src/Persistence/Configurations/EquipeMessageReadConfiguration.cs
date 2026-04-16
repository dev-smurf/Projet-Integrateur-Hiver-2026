using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EquipeMessageReadConfiguration : IEntityTypeConfiguration<EquipeMessageRead>
{
    public void Configure(EntityTypeBuilder<EquipeMessageRead> builder)
    {
        builder.HasKey(r => new { r.EquipeMessageId, r.UserId });

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
