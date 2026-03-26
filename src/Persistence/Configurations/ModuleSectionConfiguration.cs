using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ModuleSectionConfiguration : IEntityTypeConfiguration<ModuleSection>
{
    public void Configure(EntityTypeBuilder<ModuleSection> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Content)
            .HasColumnType("nvarchar(max)");

        builder.Property(s => s.SortOrder)
            .IsRequired();

        builder.HasOne(s => s.Module)
            .WithMany(m => m.Sections)
            .HasForeignKey(s => s.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.ModuleId, s.SortOrder });
    }
}
