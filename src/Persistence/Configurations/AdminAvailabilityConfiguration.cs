using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class AdminAvailabilityConfiguration : IEntityTypeConfiguration<AdminAvailability>
{
    public void Configure(EntityTypeBuilder<AdminAvailability> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.DayOfWeek).HasConversion<int>();

        builder.HasOne(a => a.Admin)
            .WithMany()
            .HasForeignKey(a => a.AdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
