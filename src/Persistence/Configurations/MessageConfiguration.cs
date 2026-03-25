using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).HasConversion<int>().HasDefaultValue(MessageType.Text);

        builder.HasOne(m => m.Expediteur).WithMany().HasForeignKey(m => m.ExpediteurId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.Receveur).WithMany().HasForeignKey(m => m.ReceveurId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.Appointment).WithMany().HasForeignKey(m => m.AppointmentId).OnDelete(DeleteBehavior.SetNull);
    }
}