using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MemberModuleConfiguration : IEntityTypeConfiguration<MemberModule>
{
    public void Configure(EntityTypeBuilder<MemberModule> builder)
    {
        builder.HasKey(mm => mm.Id);

        builder.HasOne(mm => mm.Member)
            .WithMany(m => m.MemberModules)
            .HasForeignKey(mm => mm.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mm => mm.Module)
            .WithMany(m => m.MemberModules)
            .HasForeignKey(mm => mm.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(mm => mm.ProgressPercent)
            .HasDefaultValue(0);

        builder.Property(mm => mm.IsCompleted)
            .HasDefaultValue(false);

        builder.HasIndex(mm => new { mm.MemberId, mm.ModuleId })
            .IsUnique()
            .HasFilter("Deleted IS NULL");
    }
}
