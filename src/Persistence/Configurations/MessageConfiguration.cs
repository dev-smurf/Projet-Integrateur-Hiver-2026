using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class MessageConfiguration : IEntityTypeConfiguration<Message>
{ 
    public void Configure(EntityTypeBuilder<Message> builder) 
    { 
        builder.HasKey(m => m.Id); builder.HasOne(m => m.Expediteur).WithMany().HasForeignKey(m => m.ExpediteurId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.Receveur).WithMany().HasForeignKey(m => m.ReceveurId).OnDelete(DeleteBehavior.Restrict); 
    } 
}