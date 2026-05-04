using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class EquipeConversationConfiguration : IEntityTypeConfiguration<EquipeConversation>
{
    public void Configure(EntityTypeBuilder<EquipeConversation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Equipe)
            .WithMany()
            .HasForeignKey(c => c.EquipeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.EquipeConversation)
            .HasForeignKey(m => m.EquipeConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
