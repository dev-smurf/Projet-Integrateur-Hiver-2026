using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MemberModuleSectionProgressConfiguration : IEntityTypeConfiguration<MemberModuleSectionProgress>
{
    public void Configure(EntityTypeBuilder<MemberModuleSectionProgress> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.HasOne(sp => sp.MemberModule)
            .WithMany(mm => mm.SectionProgress)
            .HasForeignKey(sp => sp.MemberModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sp => sp.ModuleSection)
            .WithMany()
            .HasForeignKey(sp => sp.ModuleSectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sp => sp.IsRead)
            .HasDefaultValue(false);

        builder.HasIndex(sp => new { sp.MemberModuleId, sp.ModuleSectionId })
            .IsUnique()
            .HasFilter("Deleted IS NULL");
    }
}
