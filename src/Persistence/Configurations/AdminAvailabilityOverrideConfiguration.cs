using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AdminAvailabilityOverrideConfiguration : IEntityTypeConfiguration<AdminAvailabilityOverride>
{
    public void Configure(EntityTypeBuilder<AdminAvailabilityOverride> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Admin)
            .WithMany()
            .HasForeignKey(a => a.AdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
