using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MemberEquipeConfiguration : IEntityTypeConfiguration<MemberEquipe>
{
    public void Configure(EntityTypeBuilder<MemberEquipe> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.MemberId, x.EquipeId })
            .IsUnique();

        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Equipe)
            .WithMany(e => e.MemberEquipes)
            .HasForeignKey(x => x.EquipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}